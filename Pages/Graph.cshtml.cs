using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Singleton.Pages;
public class GraphModel : PageModel
{
    public List<Tuple<float, float>> Values = [];
    public void OnGet()
    {
        Values.Add(new Tuple<float,float>(1, 1));
        Values.Add(new Tuple<float,float>(2, 1));
        Values.Add(new Tuple<float,float>(3, 4));
    }
}
