
using Domain.Constants;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations.SeedConfiguration
{
    public class CurrenciesSeeder : IEntityTypeConfiguration<Currency>
    {
        private readonly DateTimeOffset _defaultCreateDate = new(2023, 02, 22, 00, 00, 00,
            TimeSpan.Zero);

        public void Configure(EntityTypeBuilder<Currency> builder)
        {

            builder.ToTable("Currencies");
            builder.HasData(
                new Currency
                {
                   Id = 1,
                    Code = "RWF",
                    Description = "Rwandan franc ",
                    CreatedOn = _defaultCreateDate,
                    LastModifiedOn = _defaultCreateDate
                },
                new Currency
                {
                   Id= 2,
                    Code = "KES",
                    Description = "Kenyan Shilling",
                    CreatedOn = _defaultCreateDate,
                    LastModifiedOn = _defaultCreateDate
                     
                },
                new Currency
                {
                    Id=3,
                    Code = "UGX",
                    Description = "Ugandan Shilling",
                    CreatedOn = _defaultCreateDate,
                    LastModifiedOn = _defaultCreateDate
                },
                new Currency
                {
                    Id=4,
                    Code = "TZS",
                    Description = "Tanzanian shilling",
                    CreatedOn = _defaultCreateDate,
                    LastModifiedOn = _defaultCreateDate
                });
        }
    }

}
