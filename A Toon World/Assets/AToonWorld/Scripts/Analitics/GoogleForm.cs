using System.Collections.Generic;

public class GoogleForm 
{
    private string _id;
    public string ID => $"https://docs.google.com/forms/u/0/d/e/{_id}/formResponse";
    public string[] Fields => new string[] { "entry.1195978271", "entry.990960823", "entry.1600303878", "entry.1309892620", "entry.2129262794", "entry.457954646", "entry.1853613128", "entry.2031163248", "entry.1257877847", "entry.1598959833", "entry.825847616"};

    private GoogleForm(string id)
    {
        _id = id;
    }

    public static GoogleForm Development => new GoogleForm("1FAIpQLSfnUUwmk5rI8CJO47DiFcpG0qE6mqTvW6AQQAP8LRCg6iazgA");
    public static GoogleForm Production => new GoogleForm("1FAIpQLSfKLWt075KuBHxY-fC9D2HinfljnsomrRLlhdPOiAP002KTmA");

    public List<(string, string)> CreateForm(Analitic analitic)
    {
        List<(string, string)> pairs = new List<(string, string)>();
        pairs.Add((Fields[0], analitic.user.ToString()));
        pairs.Add((Fields[1], analitic.level));
        pairs.Add((Fields[2], analitic.game.ToString()));
        pairs.Add((Fields[3], analitic.dateTime.ToString("dd/MM/yy HH:mm:ss")));
        pairs.Add((Fields[4], analitic.eventName.ToString()));
        for (int i = 0; i < analitic.value.Length; i++)
            pairs.Add((Fields[i + 5], analitic.value[i]));
        return pairs;
    }
}