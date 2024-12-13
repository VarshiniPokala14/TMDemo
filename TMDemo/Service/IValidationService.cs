namespace TrekMasters.Service
{
    public interface IValidationService
    {
        bool IsEmailAlreadyAdded(IEnumerable<ParticipantViewModel> participants, string email);
        bool IsParticipantIndexValid(int index, int participantCount);
    }
}
