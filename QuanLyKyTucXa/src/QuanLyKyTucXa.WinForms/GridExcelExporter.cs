using System.IO.Compression;
using System.Text;
using System.Xml;

namespace QuanLyKyTucXa.WinForms;

internal static class GridExcelExporter
{
    public static void Export(DataGridView grid, string path)
    {
        var columns = grid.Columns.Cast<DataGridViewColumn>().Where(c => c.Visible)
            .OrderBy(c => c.DisplayIndex).ToArray();
        using var stream = new FileStream(path, FileMode.Create, FileAccess.Write);
        using var zip = new ZipArchive(stream, ZipArchiveMode.Create);
        Write(zip, "[Content_Types].xml", """<?xml version="1.0" encoding="utf-8"?><Types xmlns="http://schemas.openxmlformats.org/package/2006/content-types"><Default Extension="rels" ContentType="application/vnd.openxmlformats-package.relationships+xml"/><Default Extension="xml" ContentType="application/xml"/><Override PartName="/xl/workbook.xml" ContentType="application/vnd.openxmlformats-officedocument.spreadsheetml.sheet.main+xml"/><Override PartName="/xl/worksheets/sheet1.xml" ContentType="application/vnd.openxmlformats-officedocument.spreadsheetml.worksheet+xml"/></Types>""");
        Write(zip, "_rels/.rels", """<?xml version="1.0" encoding="utf-8"?><Relationships xmlns="http://schemas.openxmlformats.org/package/2006/relationships"><Relationship Id="rId1" Type="http://schemas.openxmlformats.org/officeDocument/2006/relationships/officeDocument" Target="xl/workbook.xml"/></Relationships>""");
        Write(zip, "xl/workbook.xml", """<?xml version="1.0" encoding="utf-8"?><workbook xmlns="http://schemas.openxmlformats.org/spreadsheetml/2006/main" xmlns:r="http://schemas.openxmlformats.org/officeDocument/2006/relationships"><sheets><sheet name="Du lieu" sheetId="1" r:id="rId1"/></sheets></workbook>""");
        Write(zip, "xl/_rels/workbook.xml.rels", """<?xml version="1.0" encoding="utf-8"?><Relationships xmlns="http://schemas.openxmlformats.org/package/2006/relationships"><Relationship Id="rId1" Type="http://schemas.openxmlformats.org/officeDocument/2006/relationships/worksheet" Target="worksheets/sheet1.xml"/></Relationships>""");
        using var sheet = zip.CreateEntry("xl/worksheets/sheet1.xml").Open();
        using var xml = XmlWriter.Create(sheet, new XmlWriterSettings { Encoding = new UTF8Encoding(false) });
        const string ns = "http://schemas.openxmlformats.org/spreadsheetml/2006/main";
        xml.WriteStartDocument();
        xml.WriteStartElement("worksheet", ns);
        xml.WriteStartElement("sheetData", ns);
        void Row(IEnumerable<string> values)
        {
            xml.WriteStartElement("row", ns);
            foreach (var value in values)
            {
                xml.WriteStartElement("c", ns);
                xml.WriteAttributeString("t", "inlineStr");
                xml.WriteStartElement("is", ns);
                xml.WriteElementString("t", ns, value);
                xml.WriteEndElement();
                xml.WriteEndElement();
            }
            xml.WriteEndElement();
        }
        Row(columns.Select(c => c.HeaderText));
        foreach (var row in grid.Rows.Cast<DataGridViewRow>().Where(r => r.Visible && !r.IsNewRow))
            Row(columns.Select(c => Convert.ToString(row.Cells[c.Index].FormattedValue) ?? ""));
        xml.WriteEndElement();
        xml.WriteEndElement();
        xml.WriteEndDocument();
    }

    private static void Write(ZipArchive zip, string path, string content)
    {
        using var writer = new StreamWriter(zip.CreateEntry(path).Open(), new UTF8Encoding(false));
        writer.Write(content);
    }
}
