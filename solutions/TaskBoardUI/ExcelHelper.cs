using System.Text;

namespace TfsWorkbench.TaskBoardUI
{
    using System.Drawing;
    using System.Xml;

    using OfficeOpenXml;
    using OfficeOpenXml.Drawing;
    using OfficeOpenXml.Style;

    using TfsWorkbench.Core.Helpers;
    using TfsWorkbench.Core.Interfaces;

    public static class ExcelHelper
    {
        public const int StoryStartRow = 0;
        public const int StoryRowSize = 7;
        public const int StoryStartColumn = 0;
        public const int StoryEndColumn = 0;
        public const string StoryColor = "FFFF00";
        public static int Id = 10000;

        public static void AddDrawing(this ExcelWorksheet sheet, string drawXml)
        {
            var child = sheet.Drawings.DrawingXml.LastChild;
            if (child == null)
            {
                sheet.Drawings.DrawingXml.LoadXml("<xdr:wsDr xmlns:xdr=\"http://schemas.openxmlformats.org/drawingml/2006/spreadsheetDrawing\" xmlns:a=\"http://schemas.openxmlformats.org/drawingml/2006/main\"></xdr:wsDr>");
                child = sheet.Drawings.DrawingXml.LastChild;
            }

            child.InnerXml += drawXml;

            sheet.Drawings.AddDrawing(ExcelDrawing.GetDrawing(sheet.Drawings, child.LastChild));
        }

        public static void CreateTask(this ExcelWorksheet sheet, int line, int column, IWorkbenchItem item)
        {
            var text = "(" + item.GetId() + ") " + item.GetCaption() + " - " + item.GetMetric() + "h";

            var cell = sheet.Cells[line, column];
            cell.Value = item.GetCaption() + " - " + item.GetMetric() + "h";

            if (text.ToLowerInvariant().Contains("[np]"))
            {
                cell.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(255, 255, 0, 0)); 
                cell.Style.Font.Color.SetColor(Color.White);
            }
            else
            {
                cell.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(255, 255, 255, 0));
            }

            cell.Style.WrapText = true;
            cell.Style.Border.BorderAround(ExcelBorderStyle.Thin);
            cell.Style.VerticalAlignment = ExcelVerticalAlignment.Top;
        }

        public static string CreateRowLine(int row, int size)
        {
            return string.Format(
                    "<xdr:twoCellAnchor><xdr:from><xdr:col>0</xdr:col><xdr:colOff>95250</xdr:colOff><xdr:row>{0}</xdr:row><xdr:rowOff>1733550</xdr:rowOff></xdr:from><xdr:to><xdr:col>{1}</xdr:col><xdr:colOff>1047750</xdr:colOff><xdr:row>{2}</xdr:row><xdr:rowOff>10834</xdr:rowOff></xdr:to><xdr:cxnSp macro=\"\"><xdr:nvCxnSpPr><xdr:cNvPr id=\"{3}\" name=\"Shape {3}\"/><xdr:cNvCxnSpPr/></xdr:nvCxnSpPr><xdr:spPr><a:xfrm flipV=\"1\"><a:off x=\"95250\" y=\"13468350\"/><a:ext cx=\"70675500\" cy=\"48934\"/></a:xfrm><a:prstGeom prst=\"line\"><a:avLst/></a:prstGeom><a:ln w=\"38100\"/></xdr:spPr><xdr:style><a:lnRef idx=\"1\"><a:schemeClr val=\"accent1\"/></a:lnRef><a:fillRef idx=\"0\"><a:schemeClr val=\"accent1\"/></a:fillRef><a:effectRef idx=\"0\"><a:schemeClr val=\"accent1\"/></a:effectRef><a:fontRef idx=\"minor\"><a:schemeClr val=\"tx1\"/></a:fontRef></xdr:style></xdr:cxnSp><xdr:clientData/></xdr:twoCellAnchor>",
                    row, size, row + 1, Id
                    );
        }

        public static string CreateStoryDraw(int index, string title, string body, string estimate)
        {
            var shapeId = Id++;

            var sb = new StringBuilder();
            // begin drawing
            //sb.Append("<node xmlns:xdr=\"http://schemas.openxmlformats.org/drawingml/2006/spreadsheetDrawing\" xmlns:a=\"http://schemas.openxmlformats.org/drawingml/2006/main\">");

            sb.Append("<xdr:twoCellAnchor>");

            // celula de origem e margem interna
            sb.AppendFormat(
                "<xdr:from><xdr:col>{0}</xdr:col><xdr:colOff>200025</xdr:colOff><xdr:row>{1}</xdr:row><xdr:rowOff>438150</xdr:rowOff></xdr:from>",
                StoryStartColumn, (index * StoryRowSize) + 1);

            // celula de destino e margem interna
            sb.AppendFormat(
                "<xdr:to><xdr:col>{0}</xdr:col><xdr:colOff>7510463</xdr:colOff><xdr:row>{1}</xdr:row><xdr:rowOff>952500</xdr:rowOff></xdr:to>",
                StoryEndColumn, ((index + 1) * StoryRowSize));

            // inicio do shape
            sb.Append("<xdr:sp macro=\"\" textlink=\"\">");

            //non visual properties for shape
            sb.AppendFormat("<xdr:nvSpPr><xdr:cNvPr id=\"{0}\" name=\"Shape {0}\"/><xdr:cNvSpPr/></xdr:nvSpPr>", shapeId);

            // shape properyies
            sb.AppendFormat("<xdr:spPr><a:xfrm><a:off x=\"200025\" y=\"1543050\"/><a:ext cx=\"7310438\" cy=\"11258550\"/></a:xfrm><a:prstGeom prst=\"foldedCorner\"><a:avLst/></a:prstGeom><a:solidFill><a:srgbClr val=\"{0}\"/></a:solidFill></xdr:spPr>",
                StoryColor);

            // style
            sb.Append(
                "<xdr:style><a:lnRef idx=\"2\"><a:schemeClr val=\"accent1\"><a:shade val=\"50000\"/></a:schemeClr></a:lnRef><a:fillRef idx=\"1\"><a:schemeClr val=\"accent1\"/></a:fillRef><a:effectRef idx=\"0\"><a:schemeClr val=\"accent1\"/></a:effectRef><a:fontRef idx=\"minor\"><a:schemeClr val=\"lt1\"/></a:fontRef></xdr:style>");

            // inicio do corpo (texto)
            sb.Append("<xdr:txBody>");

            // estilos de texto
            sb.Append(
                "<a:bodyPr wrap=\"square\" rtlCol=\"0\" anchor=\"t\" anchorCtr=\"0\"/><a:lstStyle><a:defPPr><a:defRPr lang=\"pt-BR\"/></a:defPPr><a:lvl1pPr marL=\"0\" algn=\"l\" defTabSz=\"914400\" rtl=\"0\" eaLnBrk=\"1\" latinLnBrk=\"0\" hangingPunct=\"1\"><a:defRPr sz=\"1800\" kern=\"1200\"><a:solidFill><a:schemeClr val=\"lt1\"/></a:solidFill><a:latin typeface=\"+mn-lt\"/><a:ea typeface=\"+mn-ea\"/><a:cs typeface=\"+mn-cs\"/></a:defRPr></a:lvl1pPr><a:lvl2pPr marL=\"457200\" algn=\"l\" defTabSz=\"914400\" rtl=\"0\" eaLnBrk=\"1\" latinLnBrk=\"0\" hangingPunct=\"1\"><a:defRPr sz=\"1800\" kern=\"1200\"><a:solidFill><a:schemeClr val=\"lt1\"/></a:solidFill><a:latin typeface=\"+mn-lt\"/><a:ea typeface=\"+mn-ea\"/><a:cs typeface=\"+mn-cs\"/></a:defRPr></a:lvl2pPr><a:lvl3pPr marL=\"914400\" algn=\"l\" defTabSz=\"914400\" rtl=\"0\" eaLnBrk=\"1\" latinLnBrk=\"0\" hangingPunct=\"1\"><a:defRPr sz=\"1800\" kern=\"1200\"><a:solidFill><a:schemeClr val=\"lt1\"/></a:solidFill><a:latin typeface=\"+mn-lt\"/><a:ea typeface=\"+mn-ea\"/><a:cs typeface=\"+mn-cs\"/></a:defRPr></a:lvl3pPr><a:lvl4pPr marL=\"1371600\" algn=\"l\" defTabSz=\"914400\" rtl=\"0\" eaLnBrk=\"1\" latinLnBrk=\"0\" hangingPunct=\"1\"><a:defRPr sz=\"1800\" kern=\"1200\"><a:solidFill><a:schemeClr val=\"lt1\"/></a:solidFill><a:latin typeface=\"+mn-lt\"/><a:ea typeface=\"+mn-ea\"/><a:cs typeface=\"+mn-cs\"/></a:defRPr></a:lvl4pPr><a:lvl5pPr marL=\"1828800\" algn=\"l\" defTabSz=\"914400\" rtl=\"0\" eaLnBrk=\"1\" latinLnBrk=\"0\" hangingPunct=\"1\"><a:defRPr sz=\"1800\" kern=\"1200\"><a:solidFill><a:schemeClr val=\"lt1\"/></a:solidFill><a:latin typeface=\"+mn-lt\"/><a:ea typeface=\"+mn-ea\"/><a:cs typeface=\"+mn-cs\"/></a:defRPr></a:lvl5pPr><a:lvl6pPr marL=\"2286000\" algn=\"l\" defTabSz=\"914400\" rtl=\"0\" eaLnBrk=\"1\" latinLnBrk=\"0\" hangingPunct=\"1\"><a:defRPr sz=\"1800\" kern=\"1200\"><a:solidFill><a:schemeClr val=\"lt1\"/></a:solidFill><a:latin typeface=\"+mn-lt\"/><a:ea typeface=\"+mn-ea\"/><a:cs typeface=\"+mn-cs\"/></a:defRPr></a:lvl6pPr><a:lvl7pPr marL=\"2743200\" algn=\"l\" defTabSz=\"914400\" rtl=\"0\" eaLnBrk=\"1\" latinLnBrk=\"0\" hangingPunct=\"1\"><a:defRPr sz=\"1800\" kern=\"1200\"><a:solidFill><a:schemeClr val=\"lt1\"/></a:solidFill><a:latin typeface=\"+mn-lt\"/><a:ea typeface=\"+mn-ea\"/><a:cs typeface=\"+mn-cs\"/></a:defRPr></a:lvl7pPr><a:lvl8pPr marL=\"3200400\" algn=\"l\" defTabSz=\"914400\" rtl=\"0\" eaLnBrk=\"1\" latinLnBrk=\"0\" hangingPunct=\"1\"><a:defRPr sz=\"1800\" kern=\"1200\"><a:solidFill><a:schemeClr val=\"lt1\"/></a:solidFill><a:latin typeface=\"+mn-lt\"/><a:ea typeface=\"+mn-ea\"/><a:cs typeface=\"+mn-cs\"/></a:defRPr></a:lvl8pPr><a:lvl9pPr marL=\"3657600\" algn=\"l\" defTabSz=\"914400\" rtl=\"0\" eaLnBrk=\"1\" latinLnBrk=\"0\" hangingPunct=\"1\"><a:defRPr sz=\"1800\" kern=\"1200\"><a:solidFill><a:schemeClr val=\"lt1\"/></a:solidFill><a:latin typeface=\"+mn-lt\"/><a:ea typeface=\"+mn-ea\"/><a:cs typeface=\"+mn-cs\"/></a:defRPr></a:lvl9pPr></a:lstStyle>");

            // texto
            sb.AppendFormat(
                "<a:p><a:pPr marL=\"0\" marR=\"0\" lvl=\"0\" indent=\"0\" algn=\"l\" defTabSz=\"914400\" rtl=\"0\" eaLnBrk=\"1\" fontAlgn=\"auto\" latinLnBrk=\"0\" hangingPunct=\"1\"><a:lnSpc><a:spcPct val=\"100000\"/></a:lnSpc><a:spcBef><a:spcPts val=\"0\"/></a:spcBef><a:spcAft><a:spcPts val=\"0\"/></a:spcAft><a:buClrTx/><a:buSzTx/><a:buFontTx/><a:buNone/><a:tabLst/><a:defRPr/></a:pPr><a:r><a:rPr kumimoji=\"0\" lang=\"pt-BR\" sz=\"3600\" b=\"1\" i=\"0\" u=\"none\" strike=\"noStrike\" kern=\"1200\" cap=\"none\" spc=\"0\" normalizeH=\"0\" baseline=\"0\" noProof=\"0\"><a:ln><a:noFill/></a:ln><a:solidFill><a:prstClr val=\"black\"/></a:solidFill><a:effectLst/><a:uLnTx/><a:uFillTx/><a:latin typeface=\"+mn-lt\"/><a:ea typeface=\"+mn-ea\"/><a:cs typeface=\"+mn-cs\"/></a:rPr><a:t>{0}</a:t></a:r><a:endParaRPr kumimoji=\"0\" lang=\"pt-BR\" sz=\"2800\" b=\"0\" i=\"0\" u=\"none\" strike=\"noStrike\" kern=\"1200\" cap=\"none\" spc=\"0\" normalizeH=\"0\" baseline=\"0\" noProof=\"0\"><a:ln><a:noFill/></a:ln><a:solidFill><a:prstClr val=\"black\"/></a:solidFill><a:effectLst/><a:uLnTx/><a:uFillTx/><a:latin typeface=\"+mn-lt\"/><a:ea typeface=\"+mn-ea\"/><a:cs typeface=\"+mn-cs\"/></a:endParaRPr></a:p>",
                title);

            // fim do corpo
            sb.Append("</xdr:txBody>");

            // fim do shape
            sb.Append("</xdr:sp>");

            // client data
            sb.Append("<xdr:clientData/>");

            // end drawing
            sb.Append("</xdr:twoCellAnchor>");

            //sb.Append("</node>");
            return sb.ToString();
        }
    }
}
