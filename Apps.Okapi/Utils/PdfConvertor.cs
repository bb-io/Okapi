namespace Apps.Okapi.Utils;

public class PdfConvertor
{
    public static MemoryStream ConvertPdfToDocx(Stream pdfStream)
    {
        var pdfDocument = new Aspose.Pdf.Document(pdfStream);
        var docxStream = new MemoryStream();
    
        pdfDocument.Save(docxStream, Aspose.Pdf.SaveFormat.DocX);
    
        docxStream.Position = 0;
        return docxStream;
    }
    
    public MemoryStream ConvertDocxToPdf(Stream docxStream)
    {
        var docDocument = new Aspose.Words.Document(docxStream);
        var pdfStream = new MemoryStream();
    
        docDocument.Save(pdfStream, Aspose.Words.SaveFormat.Pdf);
    
        pdfStream.Position = 0;
        return pdfStream;
    }
}