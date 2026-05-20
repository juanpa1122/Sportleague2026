using System.Net.Mail;
using Microsoft.Extensions.Logging;
using SportsLeague.Domain.Entities;
using SportsLeague.Domain.Interfaces.Repositories;
using SportsLeague.Domain.Interfaces.Services;

namespace SportsLeague.Domain.Services
{
    public class SponsorService : ISponsorService
    {
        private readonly ISponsorRepository _sponsorRepository;
        private readonly ITournamentRepository _tournamentRepository;
        private readonly ITournamentSponsorRepository _tournamentSponsorRepository;
        private readonly ILogger<SponsorService> _logger;

        public SponsorService(
            ISponsorRepository sponsorRepository,
            ITournamentRepository tournamentRepository,
            ITournamentSponsorRepository tournamentSponsorRepository,
            ILogger<SponsorService> logger)
        {
            _sponsorRepository = sponsorRepository;
            _tournamentRepository = tournamentRepository;
            _tournamentSponsorRepository = tournamentSponsorRepository;
            _logger = logger;
        }

        public async Task<IEnumerable<Sponsor>> GetAllAsync()
        {
            _logger.LogInformation("Retrieving all sponsors");
            return await _sponsorRepository.GetAllAsync();
        }

        public async Task<Sponsor?> GetByIdAsync(int id)
        {
            _logger.LogInformation("Retrieving sponsor with ID: {SponsorId}", id);
            var sponsor = await _sponsorRepository.GetByIdAsync(id);

            if (sponsor == null)
            {
                _logger.LogWarning("Sponsor with ID {SponsorId} not found", id);
            }

            return sponsor;
        }

        public async Task<Sponsor> CreateAsync(Sponsor sponsor)
        {
            NormalizeSponsorData(sponsor);
            ValidateSponsorData(sponsor);

            if (await _sponsorRepository.ExistsByNameAsync(sponsor.Name))
            {
                throw new InvalidOperationException(
                    $"Ya existe un patrocinador con el nombre '{sponsor.Name}'");
            }

            _logger.LogInformation("Creating sponsor: {SponsorName}", sponsor.Name);
            return await _sponsorRepository.CreateAsync(sponsor);
        }

        public async Task UpdateAsync(int id, Sponsor sponsor)
        {
            var existing = await _sponsorRepository.GetByIdAsync(id);
            if (existing == null)
            {
                throw new KeyNotFoundException($"No se encontró el patrocinador con ID {id}");
            }

            NormalizeSponsorData(sponsor);
            ValidateSponsorData(sponsor);

            if (!string.Equals(existing.Name, sponsor.Name, StringComparison.OrdinalIgnoreCase)
                && await _sponsorRepository.ExistsByNameAsync(sponsor.Name))
            {
                throw new InvalidOperationException(
                    $"Ya existe un patrocinador con el nombre '{sponsor.Name}'");
            }

            existing.Name = sponsor.Name;
            existing.ContactEmail = sponsor.ContactEmail;
            existing.Phone = sponsor.Phone;
            existing.WebsiteUrl = sponsor.WebsiteUrl;
            existing.Category = sponsor.Category;
            existing.UpdatedAt = DateTime.UtcNow;

            _logger.LogInformation("Updating sponsor with ID: {SponsorId}", id);
            await _sponsorRepository.UpdateAsync(existing);
        }

        public async Task DeleteAsync(int id)
        {
            var exists = await _sponsorRepository.ExistsAsync(id);
            if (!exists)
            {
                throw new KeyNotFoundException($"No se encontró el patrocinador con ID {id}");
            }

            _logger.LogInformation("Deleting sponsor with ID: {SponsorId}", id);
            await _sponsorRepository.DeleteAsync(id);
        }

        public async Task<TournamentSponsor> LinkToTournamentAsync(int sponsorId, int tournamentId, decimal contractAmount)
        {
            var sponsorExists = await _sponsorRepository.ExistsAsync(sponsorId);
            if (!sponsorExists)
            {
                throw new KeyNotFoundException($"No se encontró el patrocinador con ID {sponsorId}");
            }

            var tournamentExists = await _tournamentRepository.ExistsAsync(tournamentId);
            if (!tournamentExists)
            {
                throw new KeyNotFoundException($"No se encontró el torneo con ID {tournamentId}");
            }

            if (contractAmount <= 0)
            {
                throw new InvalidOperationException("ContractAmount debe ser mayor a 0");
            }

            var existing = await _tournamentSponsorRepository
                .GetByTournamentAndSponsorAsync(tournamentId, sponsorId);

            if (existing != null)
            {
                throw new InvalidOperationException(
                    "Este patrocinador ya está vinculado a este torneo");
            }

            var link = new TournamentSponsor
            {
                TournamentId = tournamentId,
                SponsorId = sponsorId,
                ContractAmount = contractAmount,
                JoinedAt = DateTime.UtcNow
            };

            _logger.LogInformation(
                "Linking sponsor {SponsorId} to tournament {TournamentId}",
                sponsorId, tournamentId);

            await _tournamentSponsorRepository.CreateAsync(link);

            return (await _tournamentSponsorRepository
                .GetByTournamentAndSponsorAsync(tournamentId, sponsorId))!;
        }

        public async Task<IEnumerable<TournamentSponsor>> GetTournamentsBySponsorAsync(int sponsorId)
        {
            var sponsorExists = await _sponsorRepository.ExistsAsync(sponsorId);
            if (!sponsorExists)
            {
                throw new KeyNotFoundException($"No se encontró el patrocinador con ID {sponsorId}");
            }

            return await _tournamentSponsorRepository.GetBySponsorAsync(sponsorId);
        }

        public async Task UnlinkFromTournamentAsync(int sponsorId, int tournamentId)
        {
            var link = await _tournamentSponsorRepository
                .GetByTournamentAndSponsorAsync(tournamentId, sponsorId);

            if (link == null)
            {
                throw new KeyNotFoundException(
                    "No existe la vinculación Sponsor-Tournament indicada");
            }

            _logger.LogInformation(
                "Unlinking sponsor {SponsorId} from tournament {TournamentId}",
                sponsorId, tournamentId);

            await _tournamentSponsorRepository.DeleteAsync(link.Id);
        }

        private static void NormalizeSponsorData(Sponsor sponsor)
        {
            sponsor.Name = sponsor.Name.Trim();
            sponsor.ContactEmail = sponsor.ContactEmail.Trim();
            sponsor.Phone = string.IsNullOrWhiteSpace(sponsor.Phone) ? null : sponsor.Phone.Trim();
            sponsor.WebsiteUrl = string.IsNullOrWhiteSpace(sponsor.WebsiteUrl) ? null : sponsor.WebsiteUrl.Trim();
        }

        private static void ValidateSponsorData(Sponsor sponsor)
        {
            if (string.IsNullOrWhiteSpace(sponsor.Name))
            {
                throw new InvalidOperationException("Name es requerido");
            }

            if (!IsValidEmail(sponsor.ContactEmail))
            {
                throw new InvalidOperationException("ContactEmail debe ser un email válido");
            }
        }

        private static bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email)) return false;
            try
            {
                _ = new MailAddress(email);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
