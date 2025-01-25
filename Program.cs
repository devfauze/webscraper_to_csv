using HtmlAgilityPack;

class Program
{
    static async Task Main(string[] args)
    {
        string url = "http://quotes.toscrape.com/";
        Console.WriteLine("Procurando por citações");

        try
        {
            var html = await GetHtmlAsync(url);
            ProcessHtml(html);
            Console.WriteLine("Busca finalizada!");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro: {ex.Message}");
        }
    }

    static async Task<string> GetHtmlAsync(string url)
    {
        using var client = new HttpClient();
        var response = await client.GetAsync(url);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadAsStringAsync();
    }

    static void ProcessHtml(string html)
    {
        var doc = new HtmlDocument();
        doc.LoadHtml(html);

        var quotes = doc.DocumentNode.SelectNodes("//div[@class='quote']");
        if (quotes == null)
        {
            Console.WriteLine("Nenhuma citação encontrada.");
            return;
        }

        var lines = new List<string> { "Citação,Autor" };

        foreach (var quote in quotes)
        {
            var text = quote.SelectSingleNode(".//span[@class='text']").InnerText.Trim().Replace(",", " ");
            var author = quote.SelectSingleNode(".//span/small[@class='author']").InnerText.Trim().Replace(",", " ");

            if (text != null && author != null)
            {
                lines.Add($"\"{text}\",\"{author}\"");
            }
            
            Console.WriteLine($"Citação: {text}");
            Console.WriteLine($"Author: {author}");
            Console.WriteLine(new string('-', 40));
        }
        
        SaveToCsv("quotes.csv", lines);
    }

    static void SaveToCsv(string filePath, List<string> lines)
    {
        try
        {
            File.WriteAllLines(filePath, lines.ToArray());
            Console.WriteLine($"Dados salvos com sucesso em: {filePath}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro: {ex.Message}");
        }
    }
}