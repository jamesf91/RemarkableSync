using Microsoft.Office.Interop.OneNote;
using OneNoteObjectModel;
using RemarkableSync.MyScript;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Windows.Ink;
using System.Windows.Input;
using System.Xml.Linq;
using System.Xml.Serialization;
using static System.Collections.Specialized.BitVector32;

namespace RemarkableSync.OnenoteAddin
{
    public class OneNoteHelper: OneNoteApplication
    {
        static private int PageXOffset = 36;
        static private int PageYOffset = 86;
        static private int ImageGap = 50;
        static private string PositionElementName = "Position";
        static private string SizeElementName = "Size";

        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        public OneNoteHelper(Application application): base(application)
        {
        }

        public string GetCurrentNotebookId()
        {
            return GetCurrentNotebook()?.ID ?? null;
        }

        private Notebook GetCurrentNotebook()
        {
            string xmlHierarchy;

            var notebooks = GetNotebooks();
            if (notebooks.Notebook.Length > 0)
            {
                var currentNoteBooks = from notebookNode in notebooks.Notebook
                                       where notebookNode.isCurrentlyViewed
                                       select notebookNode;
                return currentNoteBooks?.ElementAt(0) ?? null;
            }

            Logger.Debug("No notebook found as current");
            return null;
        }

        private OneNoteObjectModel.Section GetCurrentSection()
        {
            Notebook notebook = GetCurrentNotebook();
            
            if(notebook != null) { 
                notebook.Section = GetSections(notebook)?.ToArray();
                if (notebook.Section?.Length > 0)
                {
                    var currentSection = from sectionNode in notebook.Section
                                         where sectionNode.isCurrentlyViewed
                                         select sectionNode;

                    return currentSection?.ElementAt(0) ?? null;
                }
            }
            

            Logger.Debug("No section found as current");
            return null;
        }

        public string GetCurrentSectionId()
        {
            return GetCurrentSection()?.ID ?? null;
        }

        public Page AddPageAfterCurrent(string name)
        {
            return CreatePage(GetCurrentSectionId(), name);
        }

        public Page CreatePage(string sectionId, string pageName)
        {
            // Create the new page
            string pageId = String.Empty;
            InteropApplication.CreateNewPage(sectionId, out pageId);
            var page = GetPageContent(pageId);
            (page.Title.OE.First().Items.First() as TextRange).Value = pageName;
            return UpdatePage(page);
        }

        public void AddPageContent(string pageId, string content)
        {
            string xml;
            Page p = GetPageContent(pageId);
           

            List<OE> oeChildren = new List<OE>();
            var contentLines = content.Split('\n').ToList();
            foreach (string contentLine in contentLines)
            {
                TextRange t = new TextRange { Value = contentLine };

                OE outlineElement = new OE { 
                    Items = new TextRange[] { t } 
                };
                oeChildren.Add(outlineElement);
            }
            
            Outline o = new Outline { 
                OEChildren = new OEChildren[] {
                    new OEChildren {
                        Items = oeChildren.ToArray() 
                    }
                } 
            };
            p.Items = new PageObject[] { o };

            // Update the page
            UpdatePage(p);
        }

        public void AppendPageImages(string pageId, List<Bitmap> images, double zoom = 1.0)
        {
            string xml;
            InteropApplication.GetPageContent(pageId, out xml, PageInfo.piAll, XMLSchema.xs2013);
            var pageDoc = XDocument.Parse(xml);

            int yPos = GetBottomContentYPos(pageDoc);

            foreach (var image in images)
            {
                yPos = AppendImage(pageDoc, image, zoom, yPos) + ImageGap;
            }

            InteropApplication.UpdatePageContent(pageDoc.ToString(), DateTime.MinValue, XMLSchema.xs2013);
        }

        public void AppendPageShapeFromMyScriptRequest(string pageId, HwrRequestBundle bundle, double zoom = 1.0)
        {
            OneNoteApplication oneNote = new OneNoteApplication(InteropApplication);
            Page p = oneNote.GetPageContent(pageId);
            if (p.Items == null)
            {
                p.Items = new PageObject[1];
            }
            int yPos = GetBottomContentYPos(p);
            int maxX = 0;
            int maxY = 0;

            foreach (var strokeGroup in bundle.Request.strokeGroups)
            {

                StrokeCollection strokes = new StrokeCollection();
                foreach (var stroke in strokeGroup.strokes)
                {
                    StylusPointCollection points = new StylusPointCollection();
                    for (int i = 0; i < stroke.x.Length; i++)
                    {
                        points.Add(new StylusPoint(stroke.x[i], stroke.y[i]));
                        maxX = Math.Max(maxX, stroke.x[i]);
                        maxY = Math.Max(maxY, stroke.y[i]);
                    }
                    strokes.Add(new System.Windows.Ink.Stroke(points));
                }

                MemoryStream ms = new MemoryStream();
                strokes.Save(ms);

                InkDrawing drawing = new InkDrawing();
                drawing.Position = new Position();
                drawing.Position.x = 0;
                drawing.Position.y = yPos;

                drawing.Size = new OneNoteObjectModel.Size();
                drawing.Size.width = (int)Math.Round(maxX * zoom);
                drawing.Size.height = (int)Math.Round(maxY * zoom);

                drawing.Item = ms.ToArray();
                p.Items[0] = drawing;
            }
            try
            {
                String test = OneNoteApplication.XMLSerialize(p);
                oneNote.UpdatePage(p);
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                throw ex;
            }
        }
       
        public void AppendPageImage(string pageId, Bitmap image, double zoom = 1.0)
        {
            string xml;
            InteropApplication.GetPageContent(pageId, out xml, PageInfo.piAll, XMLSchema.xs2013);
            var pageDoc = XDocument.Parse(xml);

            int yPos = GetBottomContentYPos(pageDoc);
            AppendImage(pageDoc, image, zoom, yPos);

            InteropApplication.UpdatePageContent(pageDoc.ToString(), DateTime.MinValue, XMLSchema.xs2013);
        }

        private int AppendImage(XDocument pageDoc, Bitmap bitmap, double zoom, int yPos)
        {
            int height = (int)Math.Round(bitmap.Height * zoom);
            int width = (int)Math.Round(bitmap.Width * zoom);

            var ns = pageDoc.Root.Name.Namespace;
            XElement imageEl = new XElement(ns + "Image");

            XElement positionEl = new XElement(ns + "Position");
            positionEl.Add(new XAttribute("x", PageXOffset));
            positionEl.Add(new XAttribute("y", yPos));

            XElement sizeEl = new XElement(ns + "Size");
            sizeEl.Add(new XAttribute("width", width));
            sizeEl.Add(new XAttribute("height", height));

            XElement dataEl = new XElement(ns + "Data");
            MemoryStream stream = new MemoryStream();
            bitmap.Save(stream, ImageFormat.Png);
            dataEl.Value = Convert.ToBase64String(stream.ToArray());

            imageEl.Add(positionEl);
            imageEl.Add(sizeEl);
            imageEl.Add(dataEl);

            pageDoc.Root.Add(imageEl);
            return (yPos + height);
        }

        private int GetBottomContentYPos(Page page)
        {
            int lowestYPos = PageYOffset;
            if(page.Items == null)
            {
                return lowestYPos;
            }
            foreach (var item in page.Items)
            {
                if (item == null || item.Position == null || item.Size == null)
                {
                    continue;
                }

                try
                {
                    int yPos = (int)item.Position.y;;
                    int height = (int)item.Size.height;
                    lowestYPos = Math.Max(lowestYPos, (yPos + height));
                }
                catch (Exception err)
                {
                    Logger.Error($"error: {err.Message}");
                    continue;
                }
            }
            return lowestYPos;
        }

        private int GetBottomContentYPos(XDocument pageDoc)
        {
            var ns = pageDoc.Root.Name.Namespace;
            int lowestYPos = PageYOffset;

            foreach (var child in pageDoc.Root.Elements())
            {
                var posEl = child.Element(ns + PositionElementName);
                var sizeEl = child.Element(ns + SizeElementName);
                if (posEl == null || sizeEl == null)
                {
                    continue;
                }

                try
                {
                    int yPos = 0;
                    int height = 0;
                    string yAttribValue = posEl.Attribute("y")?.Value;
                    if (yAttribValue != null)
                    {
                        yPos = (int)double.Parse(yAttribValue, CultureInfo.InvariantCulture);
                    }
                    string heightAttribValue = sizeEl.Attribute("height")?.Value;
                    if (heightAttribValue != null)
                    {
                        height = (int)double.Parse(heightAttribValue, CultureInfo.InvariantCulture);
                    }

                    lowestYPos = Math.Max(lowestYPos, (yPos + height));
                }
                catch (Exception err)
                {
                    Logger.Error($"error: {err.Message}");
                    continue;
                }
            }
            return lowestYPos;
        }
    }
}
