using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using RestSharp;
public class MesDto
{
    public int Numero { get; set; }
    public string Nombre { get; set; }
}
public class FechaHelper
{ 
       public static List<MesDto> ObtenerMeses()
    {
        return Enumerable.Range(1, 12)
            .Select(m => new MesDto
            {
                Numero = m,
                Nombre = CultureInfo.GetCultureInfo("es-ES").DateTimeFormat.GetMonthName(m)
            })
            .ToList();
    }

    public static List<int> ObtenerAnios()
    {
        int anioActual = DateTime.Now.Year;
        return Enumerable.Range(anioActual, 4).ToList();
    }
}
