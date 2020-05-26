using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;

namespace Domain
{
    public class Friend
    {

        [ForeignKey(nameof(User))]
        [InverseProperty(nameof(User))]
        public string RequesterId { get; set; } = default!;
        public User? Requester { get; set; }
        
        [ForeignKey(nameof(User))]
        [InverseProperty(nameof(User))]
        public string ResponderId { get; set; } = default!;
        public User? Responder { get; set; }
        
        public string RequesterName { get; set; } = default!;
        public string ResponderName { get; set; } = default!;
        public string Status { get; set; } = default!;
    }
}