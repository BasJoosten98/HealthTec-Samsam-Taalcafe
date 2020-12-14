using System;
using System.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Configuration;

#nullable disable

namespace Taalcafe.Models.DB
{
    public partial class dbi380705_taalcafeContext : DbContext
    {
        public dbi380705_taalcafeContext()
        {
        }

        public dbi380705_taalcafeContext(DbContextOptions<dbi380705_taalcafeContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Account> Accounts { get; set; }
        public virtual DbSet<Gebruiker> Gebruikers { get; set; }
        public virtual DbSet<Sessie> Sessies { get; set; }
        public virtual DbSet<SessiePartner> SessiePartners { get; set; }
        public virtual DbSet<Thema> Themas { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            IConfigurationRoot configuration = new ConfigurationBuilder()
            .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
            .AddJsonFile("appsettings.json")
            .Build();
            optionsBuilder.UseSqlServer(configuration.GetConnectionString("Taalcafe"));
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseSqlServer(ConfigurationManager.ConnectionStrings["Taalcafe"].ConnectionString);
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("Relational:Collation", "SQL_Latin1_General_CP1_CI_AS");

            modelBuilder.Entity<Account>(entity =>
            {
                entity.HasKey(e => e.GebruikerId);

                entity.ToTable("Account");

                entity.Property(e => e.GebruikerId)
                    .ValueGeneratedNever()
                    .HasColumnName("Gebruiker_Id");

                entity.Property(e => e.Gebruikersnaam)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Type)
                    .IsRequired()
                    .HasMaxLength(20);

                entity.Property(e => e.Wachtwoord)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.HasOne(d => d.Gebruiker)
                    .WithOne(p => p.Account)
                    .HasForeignKey<Account>(d => d.GebruikerId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Account_Gebruiker");
            });

            modelBuilder.Entity<Gebruiker>(entity =>
            {
                entity.ToTable("Gebruiker");

                entity.Property(e => e.Email)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Inlogcode).HasMaxLength(50);

                entity.Property(e => e.Naam)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Niveau)
                    .IsRequired()
                    .HasMaxLength(20);

                entity.Property(e => e.Telefoon)
                    .IsRequired()
                    .HasMaxLength(50);
            });

            modelBuilder.Entity<Sessie>(entity =>
            {
                entity.ToTable("Sessie");

                entity.Property(e => e.Datum).HasColumnType("date");

                entity.Property(e => e.ThemaId).HasColumnName("Thema_Id");

                entity.HasOne(d => d.Thema)
                    .WithMany(p => p.Sessies)
                    .HasForeignKey(d => d.ThemaId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Sessie_Thema");
            });

            modelBuilder.Entity<SessiePartner>(entity =>
            {
                entity.HasKey(e => new { e.TaalcoachId, e.CursistId, e.SessieId });

                entity.Property(e => e.TaalcoachId).HasColumnName("Taalcoach_Id");

                entity.Property(e => e.CursistId).HasColumnName("Cursist_Id");

                entity.Property(e => e.SessieId).HasColumnName("Sessie_Id");

                entity.Property(e => e.CijferCursist).HasColumnName("Cijfer_Cursist");

                entity.Property(e => e.CijferTaalcoach).HasColumnName("Cijfer_Taalcoach");

                entity.Property(e => e.FeedbackCursist).HasColumnName("Feedback_Cursist");

                entity.Property(e => e.FeedbackTaalcoach).HasColumnName("Feedback_Taalcoach");

                entity.HasOne(d => d.Cursist)
                    .WithMany(p => p.SessiePartnerCursists)
                    .HasForeignKey(d => d.CursistId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_SessiePartners_Gebruiker1");

                entity.HasOne(d => d.Sessie)
                    .WithMany(p => p.SessiePartners)
                    .HasForeignKey(d => d.SessieId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_SessiePartners_Sessie");

                entity.HasOne(d => d.Taalcoach)
                    .WithMany(p => p.SessiePartnerTaalcoaches)
                    .HasForeignKey(d => d.TaalcoachId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_SessiePartners_Gebruiker");
            });

            modelBuilder.Entity<Thema>(entity =>
            {
                entity.ToTable("Thema");

                entity.Property(e => e.Naam).HasMaxLength(50);
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
