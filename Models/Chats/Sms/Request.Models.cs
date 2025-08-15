public class SendTextResponseUltraMsg
{
    public string sent { get; set; }
    public string message { get; set; }
    public int id { get; set; }
}
#region "Send Text"
public class RequestChatSms
{
    public string codigoEmpresaCC { get; set; }
    public string mensaje { get; set; }
    public int codigoUsuarioXP { get; set; }
    public List<RequestChatSmsDestinatario> numeros { get; set; }
}
public class RequestChatSmsDestinatario
{
    public string NroTelefono { get; set; }
    public string? Nombre { get; set; }
}
#endregion
#region "Send Imagen"
public class RequestSendImage
{ 
    public string codigoEmpresaCC { get; set; }
    public string mensaje { get; set; }
    public string dataBase64 { get; set; }
    public string extension { get; set; }
    public int codigoUsuarioXP { get; set; }
    public List<RequestChatSmsDestinatario> numeros { get; set; }
}
#endregion

#region  "Send Document"
public class RequestSendDocument
{ 
    public string codigoEmpresaCC { get; set; }
    public string fileName { get; set; }
    public string dataBase64 { get; set; }
    public string extension { get; set; }
    public string mensaje { get; set; }
    public int codigoUsuarioXP { get; set; }
    public List<RequestChatSmsDestinatario> numeros { get; set; }
}
#endregion
#region "Send Video"
public class RequestSendVideo
{ 
    public string codigoEmpresaCC { get; set; }
    public string mensaje { get; set; }
    public string dataBase64 { get; set; }
    public int codigoUsuarioXP { get; set; }
    public List<RequestChatSmsDestinatario> numeros { get; set; }
}
#endregion