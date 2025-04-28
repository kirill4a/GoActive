using GoActive.Infrastructure.Storage.Geo.Entities;
using GoActive.Modules.Geo.Application.Address;
using GoActive.Modules.Geo.Application.Shared.Storage;

namespace GoActive.Infrastructure.Storage.Geo.Repositories;

internal sealed class AddressRepository(IGeoContext context, IUnitOfWork unitOfWork) : IAddressCreator
{
    public async Task<int> CreateAddresses(IReadOnlyCollection<CreateAddressDto> addresses, CancellationToken cancellation)
    {
        var dbEntities = addresses.Select(Map);
        context.Addresses.AddRange(dbEntities);

        return await unitOfWork.CommitAsync(cancellation);
    }

    private static Address Map(CreateAddressDto address)
        =>
        new()
        {
            Id = Guid.NewGuid(),
            Source = address.Source,
            ExternalId = address.ExternalId,
            Country = address.CountryCode,
            Region = address.Region,
            District = address.District,
            Settlement = address.Settlement,
            Street = address.Street,
            Building = address.Building,
            PostalCode = address.PostalCode,
            Hash = address.Hash,
            Location = address.Location,
            IsActive = true,
        };
}
