using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreElements;
public class HttpHelper
{
    private readonly HttpClient client = new HttpClient();

    public async Task<string> GetTextContent(string url)
    {
        return await client.GetStringAsync(url);
    }
}
