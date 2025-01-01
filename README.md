
### Overview
---
Experience with Azure AI Document Intelligence
   
### Key Aspects
---
- Azure
  - Azure AI Serivces
    - Azure AI Serivces # Document Intelligence

### Environments
---
- Microsoft Visual Studio 2022
  - .NET 9 (Standard Term Support)

### Diagrams
---
![image](https://github.com/user-attachments/assets/4ba107eb-0fa9-4443-8339-ce26ccc7c443)


### Actions
---
Create the required resources
  - In the Azure portal, select Create a resource
  - In the Search services and marketplace box, type Document Intelligence and then press Enter
  - In the Document intelligence page, select Create
  - In the Create Document intelligence page, under Project Details, select your Subscription and either select an existing Resource group or create a new one
  - Under Instance details, select a Region near your users
  - In the Name textbox, type a unique name for the resource
  - Select a Pricing tier and then select Review + create
  - If the validation tests pass, select Create. Azure deploys the new Azure AI Document Intelligence resource.

Consume Azure AI Document Intelligence Processing
  - In the Azure portal, navigate to the Azure AI Document Intelligence resource
  - Under Resource Management, select Keys and Endpoint
  - Copy either KEY 1 or KEY 2 and the Endpoint values and store them for use in your application code
  - In  Microsoft Visual Studio, create a new ConsoleApp project
  - Add the NuGet package Azure.AI.FormRecognizer
  - In Program.cs, copy the Microsoft code example
```
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
```
- Run the code

### Media
---
![image](https://github.com/user-attachments/assets/8a16bbad-d7dd-4bcd-947d-8a1668064b49)
---
![image](https://github.com/user-attachments/assets/af8b7f45-a7cd-474f-8cac-802aa4966313)
---
![image](https://github.com/user-attachments/assets/5a932fde-5862-4127-86c5-7ad3d122b8fb)
---
![image](https://github.com/user-attachments/assets/2a3c2216-d6e5-406c-ad3a-73c836de822e)

### References
---
- [Plan Azure AI Document Intelligence resources - Training | Microsoft Learn](https://learn.microsoft.com/en-us/training/modules/plan-form-recognizer-solution/3-resources?pivots=csharp)
- [Quickstart: Document Intelligence client libraries - Azure AI services | Microsoft Learn](https://learn.microsoft.com/en-us/azure/ai-services/document-intelligence/quickstarts/get-started-sdks-rest-api?view=doc-intel-3.0.0&preserve-view=true&pivots=programming-language-csharp)
