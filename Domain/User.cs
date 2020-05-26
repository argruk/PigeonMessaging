using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain
{
    public class User
    {
        public string UserId { get; set; } = default!;

        public string UserName { get; set; } = default!;
        public string? UserBio { get; set; }
        public string Email { get; set; } = default!;
        public string? ImageUrl { get; set; }
        public StatusIndicator StatusIndicator { get; set; } = default!;
        
        [InverseProperty(nameof(Friend.Requester))]
        public ICollection<Friend>? FriendRequesters { get; set; }
        [InverseProperty(nameof(Friend.Responder))]
        public ICollection<Friend>? FriendResponders { get; set; }

    }
}