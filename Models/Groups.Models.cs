public class WhatsAppGroup
{
    public string id { get; set; }
    public string name { get; set; }
    public bool isReadOnly { get; set; }
    public bool isGroup { get; set; }
    public GroupMetadata groupMetadata { get; set; }
}
public class GroupMetadata
{
    public string edit_group_info { get; set; }
    public string send_messages { get; set; }

    public List<MiembroGrupo> participants { get; set; }
}
public class MiembroGrupo
{
    public string id { get; set; }
}