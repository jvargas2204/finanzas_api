using FinanzasApi.Domain.Entities;

using FinanzasApi.Domain.Enums;

namespace FinanzasApi.Domain.Entities
{
    public class WorkspaceMember
    {
        public Guid WorkspaceId { get; set; }
        public Workspace Workspace { get; set; } = null!;

        public Guid UserId { get; set; }
        public User User { get; set; } = null!;

        public MemberRole Role { get; set; } = MemberRole.Owner;

        public DateTime JoinedAt { get; set; } = DateTime.UtcNow;
    }
}
