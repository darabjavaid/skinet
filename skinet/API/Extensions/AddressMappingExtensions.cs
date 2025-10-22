using System;
using API.DTOs;
using Core.Entities;

namespace API.Extensions;

public static class AddressMappingExtensions
{
    public static Address ToAddressEntity(this AddressDto addressDto)
    {
        ArgumentNullException.ThrowIfNull(addressDto);

        return new Address
        {
            Line1 = addressDto.Line1,
            Line2 = addressDto.Line2,
            City = addressDto.City,
            State = addressDto.State,
            ZipCode = addressDto.ZipCode,
            Country = addressDto.Country
        };
    }

    public static AddressDto? ToAddressDto(this Address? address)
    {
        if (address == null) return null;

        return new AddressDto
        {
            Line1 = address.Line1,
            Line2 = address.Line2,
            City = address.City,
            State = address.State,
            ZipCode = address.ZipCode,
            Country = address.Country
        };
    }

    public static void UpdateFromDto(this Address address, AddressDto addressDto)
    {
        ArgumentNullException.ThrowIfNull(address);
        ArgumentNullException.ThrowIfNull(addressDto);

        address.Line1 = addressDto.Line1;
        address.Line2 = addressDto.Line2;
        address.City = addressDto.City;
        address.State = addressDto.State;
        address.ZipCode = addressDto.ZipCode;
        address.Country = addressDto.Country;

    }
}
