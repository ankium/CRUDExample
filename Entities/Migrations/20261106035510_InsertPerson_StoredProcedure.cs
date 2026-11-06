using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Entities.Migrations
{
    /// <inheritdoc />
    public partial class InsertPerson_StoredProcedure : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            string sp_InsertPerson = @"
                CREATE PROCEDURE [dbo].[InsertPerson]
                    @PersonID UNIQUEIDENTIFIER,
                    @PersonName NVARCHAR(40),
                    @Email NVARCHAR(50),
                    @DateOfBirth DATETIME2(7),
                    @Gender NVARCHAR(10),
                    @CountryID UNIQUEIDENTIFIER,
                    @Address NVARCHAR(200),
                    @ReceiveNewsLetters BIT
                AS
                BEGIN
                    INSERT INTO [dbo].[Persons] (PersonID, PersonName, Email, DateOfBirth, Gender, CountryID, Address, ReceiveNewsLetters)
                    VALUES (@PersonID, @PersonName, @Email, @DateOfBirth, @Gender, @CountryID, @Address, @ReceiveNewsLetters);
                END";
            migrationBuilder.Sql(sp_InsertPerson);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            string sp_DropInsertPerson = @"DROP PROCEDURE [dbo].[InsertPerson]";
            migrationBuilder.Sql(sp_DropInsertPerson);
        }
    }
}
