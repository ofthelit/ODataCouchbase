using System.ComponentModel.DataAnnotations;
using Couchbase.Linq.Filters;
using Newtonsoft.Json;

/// <summary>
/// Copied from https://gist.github.com/jeffrymorris/c3bf85d73a1e7dfcc5f25f4e581d689a
/// </summary>
[DocumentTypeFilter("airline")]
public class AirLine
{
    public string Id { get; set; }
    public string Type { get; set; }
    public string Name { get; set; }
    public string Iata { get; set; }
    public string Icao { get; set; }
    public string Callsign { get; set; }
    public string Country { get; set; }
    public override string ToString()
    {
        return JsonConvert.SerializeObject(this);
    }
}