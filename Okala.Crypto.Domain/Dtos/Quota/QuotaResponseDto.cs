using System.Runtime.Serialization;

namespace Okala.Crypto.Domain.Dtos.Quota;

[DataContract]
public class QuotaResponseDto
{
    [DataMember]
    public List<PricePairDto> Prices { get; set; }
}

[DataContract]
public class PricePairDto
{
    [DataMember]
    public string Currency { get; set; }
    [DataMember]
    public decimal Value { get; set; }
}