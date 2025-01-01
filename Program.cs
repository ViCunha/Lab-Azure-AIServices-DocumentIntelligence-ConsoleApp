using Azure;

using Azure.AI.FormRecognizer.DocumentAnalysis;

string endpoint = "<your-endpoint>";

string key = "<your-key>";

AzureKeyCredential credential = new AzureKeyCredential(key);

DocumentAnalysisClient client = new DocumentAnalysisClient(new Uri(endpoint), credential);

Uri fileUri = new Uri("https://raw.githubusercontent.com/Azure-Samples/cognitive-services-REST-api-samples/master/curl/form-recognizer/sample-layout.pdf");

AnalyzeDocumentOperation operation = await client.AnalyzeDocumentFromUriAsync(WaitUntil.Completed, "prebuilt-layout", fileUri);

AnalyzeResult result = operation.Value;

foreach (DocumentPage page in result.Pages)
{
    Console.WriteLine($"Document Page {page.PageNumber} has {page.Lines.Count} line(s), {page.Words.Count} word(s),");

    Console.WriteLine($"and {page.SelectionMarks.Count} selection mark(s).");

    for (int i = 0; i < page.Lines.Count; i++)
    {
        DocumentLine line = page.Lines[i];

        Console.WriteLine($"  Line {i} has content: '{line.Content}'.");

        Console.WriteLine($"    Its bounding polygon (points ordered clockwise):");

        for (int j = 0; j < line.BoundingPolygon.Count; j++)
        {
            Console.WriteLine($"      Point {j} => X: {line.BoundingPolygon[j].X}, Y: {line.BoundingPolygon[j].Y}");
        }
    }

    for (int i = 0; i < page.SelectionMarks.Count; i++)
    {
        DocumentSelectionMark selectionMark = page.SelectionMarks[i];

        Console.WriteLine($"  Selection Mark {i} is {selectionMark.State}.");

        Console.WriteLine($"    Its bounding polygon (points ordered clockwise):");

        for (int j = 0; j < selectionMark.BoundingPolygon.Count; j++)
        {
            Console.WriteLine($"      Point {j} => X: {selectionMark.BoundingPolygon[j].X}, Y: {selectionMark.BoundingPolygon[j].Y}");
        }
    }
}

Console.WriteLine("Paragraphs:");

foreach (DocumentParagraph paragraph in result.Paragraphs)
{
    Console.WriteLine($"  Paragraph content: {paragraph.Content}");

    if (paragraph.Role != null)
    {
        Console.WriteLine($"    Role: {paragraph.Role}");
    }
}

foreach (DocumentStyle style in result.Styles)
{
    bool isHandwritten = style.IsHandwritten.HasValue && style.IsHandwritten == true;

    if (isHandwritten && style.Confidence > 0.8)
    {
        Console.WriteLine($"Handwritten content found:");

        foreach (DocumentSpan span in style.Spans)
        {
            Console.WriteLine($"  Content: {result.Content.Substring(span.Index, span.Length)}");
        }
    }
}

Console.WriteLine("The following tables were extracted:");

for (int i = 0; i < result.Tables.Count; i++)
{
    DocumentTable table = result.Tables[i];
    Console.WriteLine($"  Table {i} has {table.RowCount} rows and {table.ColumnCount} columns.");

    foreach (DocumentTableCell cell in table.Cells)
    {
        Console.WriteLine($"    Cell ({cell.RowIndex}, {cell.ColumnIndex}) has kind '{cell.Kind}' and content: '{cell.Content}'.");
    }
}

