public class Sms
{
    public int SmsId { get; set; }
    public int EmpreaId { get; set; }
    public int InstanciaId { get; set; }
    public string Mensaje { get; set; }
    public DateTime FechaCreacion { get; set; } = DateTime.Now;
    public bool Estado { get; set; } = true;
    public int CodigoUsuario { get; set; }
    public string Documento { get; set; }

}

public class SmsDetails
{
    public int SmsId { get; set; }
    public int TipoSmsId { get; set; } 
    public string Mensaje { get; set; }  
    public string Documento { get; set; }
    public string Caption { get; set; }

}
public class SmsSave
{
    public int EmpreaId { get; set; }
    public int InstanciaId { get; set; }
    public int TipoSmsId { get; set; }
    public string Mensaje { get; set; }
    public int CodigoUsuario { get; set; }
    public string Documento { get; set; }
    public string Caption { get; set; }


}

public class TipoSms
{
    public int TipoSmsId { get; set; } = 0;
    public string Descripcion { get; set; } = "";

    public string Ruta { get; set; } = "";
    public string RutaXP { get; set; } = "";
    public bool Archivo { get; set; } 

    
}