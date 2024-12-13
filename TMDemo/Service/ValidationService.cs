namespace TrekMasters.Service
{
    public class ValidationService : IValidationService
    {
        public bool IsEmailAlreadyAdded(IEnumerable<ParticipantViewModel> participants, string email)
        {
            return participants.Any(p => p.Email.Equals(email, StringComparison.OrdinalIgnoreCase));
        }

        public bool IsParticipantIndexValid(int index, int participantCount)
        {
            return index >= 0 && index < participantCount;
        }
    }
}
