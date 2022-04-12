using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Configuration;
using Subby.Data;

namespace Subby.Data
{
    public partial class SubbynetworkContext : DbContext
    {
        public SubbynetworkContext()
        {
        }

        public SubbynetworkContext(DbContextOptions<SubbynetworkContext> options)
            : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var configuration = new ConfigurationBuilder()
                //.SetBasePath(Directory.GetCurrentDirectory())
                //.AddJsonFile("appsettings.json")
                .Build();

            var connectionString = configuration.GetConnectionString("SQLConnection");
            optionsBuilder.UseSqlServer("Data Source=tcp:susby.database.windows.net,1433;Initial Catalog=subbynetwork;Persist Security Info=False;User ID=subbynetwork1;Password=Sustainability123;MultipleActiveResultSets=True;encrypt=false;");
        }
        public virtual DbSet<Advert> Advert { get; set; }
        public virtual DbSet<AdvertCategory> AdvertCategory { get; set; }
        public virtual DbSet<Audit> Audit { get; set; }
        public virtual DbSet<Benefit> Benefit { get; set; }
        public virtual DbSet<BenefitSponsor> BenefitSponsor { get; set; }
        public virtual DbSet<Chat> Chat { get; set; }
        public virtual DbSet<ChatChannel> ChatChannel { get; set; }
        public virtual DbSet<ChatMember> ChatMember { get; set; }
        public virtual DbSet<Configuration> Configuration { get; set; }
        public virtual DbSet<Job> Job { get; set; }
        public virtual DbSet<JobActivity> JobActivity { get; set; }
        public virtual DbSet<JobApplication> JobApplication { get; set; }
        public virtual DbSet<JobInterest> JobInterest { get; set; }
        public virtual DbSet<JobTrade> JobTrade { get; set; }
        public virtual DbSet<Logs> Logs { get; set; }
        public virtual DbSet<Media> Media { get; set; }
        public virtual DbSet<MediaMeta> MediaMeta { get; set; }
        public virtual DbSet<Notification> Notification { get; set; }
        public virtual DbSet<QrtzBlobTriggers> QrtzBlobTriggers { get; set; }
        public virtual DbSet<QrtzCalendars> QrtzCalendars { get; set; }
        public virtual DbSet<QrtzCronTriggers> QrtzCronTriggers { get; set; }
        public virtual DbSet<QrtzFiredTriggers> QrtzFiredTriggers { get; set; }
        public virtual DbSet<QrtzJobDetails> QrtzJobDetails { get; set; }
        public virtual DbSet<QrtzLocks> QrtzLocks { get; set; }
        public virtual DbSet<QrtzPausedTriggerGrps> QrtzPausedTriggerGrps { get; set; }
        public virtual DbSet<QrtzSchedulerState> QrtzSchedulerState { get; set; }
        public virtual DbSet<QrtzSimpleTriggers> QrtzSimpleTriggers { get; set; }
        public virtual DbSet<QrtzSimpropTriggers> QrtzSimpropTriggers { get; set; }
        public virtual DbSet<QrtzTriggers> QrtzTriggers { get; set; }
        public virtual DbSet<Role> Role { get; set; }
        public virtual DbSet<Session> Session { get; set; }
        public virtual DbSet<Sponsor> Sponsor { get; set; }
        public virtual DbSet<Subscription> Subscription { get; set; }
        public virtual DbSet<ToDoItem> ToDoItem { get; set; }
        public virtual DbSet<Trade> Trade { get; set; }
        public virtual DbSet<Transaction> Transaction { get; set; }
        public virtual DbSet<User> User { get; set; }
        public virtual DbSet<UserReview> UserReview { get; set; }
        public virtual DbSet<UserToken> UserToken { get; set; }
        public virtual DbSet<UserTrade> UserTrade { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.UseCollation("Latin1_General_CI_AS");

            modelBuilder.Entity<Advert>(entity =>
            {
                entity.HasOne(d => d.Category)
                    .WithMany(p => p.Advert)
                    .HasForeignKey(d => d.CategoryId)
                    .HasConstraintName("FK_BDA1E7C8");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Advert)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("FK_370ADD2A");
            });

            modelBuilder.Entity<BenefitSponsor>(entity =>
            {
                entity.HasOne(d => d.Benefit)
                    .WithMany(p => p.BenefitSponsor)
                    .HasForeignKey(d => d.BenefitId)
                    .HasConstraintName("FK_3108400C");

                entity.HasOne(d => d.Sponsor)
                    .WithMany(p => p.BenefitSponsor)
                    .HasForeignKey(d => d.SponsorId)
                    .HasConstraintName("FK_D776BC5C");
            });

            modelBuilder.Entity<Chat>(entity =>
            {
                entity.HasOne(d => d.Channel)
                    .WithMany(p => p.Chat)
                    .HasForeignKey(d => d.ChannelId)
                    .HasConstraintName("FK_CAE0D318");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Chat)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("FK_4CB01ABF");
            });

            modelBuilder.Entity<ChatChannel>(entity =>
            {
                entity.HasOne(d => d.Advert)
                    .WithMany(p => p.ChatChannel)
                    .HasForeignKey(d => d.AdvertId)
                    .HasConstraintName("FK_B979C80B");
            });

            modelBuilder.Entity<ChatMember>(entity =>
            {
                entity.HasOne(d => d.Channel)
                    .WithMany(p => p.ChatMember)
                    .HasForeignKey(d => d.ChannelId)
                    .HasConstraintName("FK_D88ECE7F");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.ChatMember)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("FK_49B66367");
            });

            modelBuilder.Entity<Job>(entity =>
            {
                entity.HasOne(d => d.TradeId1Navigation)
                    .WithMany(p => p.Job)
                    .HasForeignKey(d => d.TradeId1)
                    .HasConstraintName("FK_4DF24D4C");

                entity.HasOne(d => d.UserId1Navigation)
                    .WithMany(p => p.Job)
                    .HasForeignKey(d => d.UserId1)
                    .HasConstraintName("FK_3847A6CD");
            });

            modelBuilder.Entity<JobActivity>(entity =>
            {
                entity.HasOne(d => d.Job)
                    .WithMany(p => p.JobActivity)
                    .HasForeignKey(d => d.JobId)
                    .HasConstraintName("FK_8426D499");
            });

            modelBuilder.Entity<JobApplication>(entity =>
            {
                entity.HasOne(d => d.Applicant)
                    .WithMany(p => p.JobApplicationApplicant)
                    .HasForeignKey(d => d.ApplicantId)
                    .HasConstraintName("FK_96083898");

                entity.HasOne(d => d.Job)
                    .WithMany(p => p.JobApplication)
                    .HasForeignKey(d => d.JobId)
                    .HasConstraintName("FK_A4A4EF56");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.JobApplicationUser)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("FK_8803D4E4");
            });

            modelBuilder.Entity<JobInterest>(entity =>
            {
                entity.HasOne(d => d.Job)
                    .WithMany(p => p.JobInterest)
                    .HasForeignKey(d => d.JobId)
                    .HasConstraintName("FK_B0CEFA54");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.JobInterest)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("FK_34B7BFD7");
            });

            modelBuilder.Entity<JobTrade>(entity =>
            {
                entity.HasOne(d => d.Job)
                    .WithMany(p => p.JobTrade)
                    .HasForeignKey(d => d.JobId)
                    .HasConstraintName("FK_11EFF80E");

                entity.HasOne(d => d.Trade)
                    .WithMany(p => p.JobTrade)
                    .HasForeignKey(d => d.TradeId)
                    .HasConstraintName("FK_6CFFC08F");
            });

            modelBuilder.Entity<Media>(entity =>
            {
                entity.HasOne(d => d.Advert)
                    .WithMany(p => p.MediaNavigation)
                    .HasForeignKey(d => d.AdvertId)
                    .HasConstraintName("FK_9F692083");
            });

            modelBuilder.Entity<MediaMeta>(entity =>
            {
                entity.HasOne(d => d.Media)
                    .WithMany(p => p.MediaMeta)
                    .HasForeignKey(d => d.MediaId)
                    .HasConstraintName("FK_AEE22AF2");
            });

            modelBuilder.Entity<Notification>(entity =>
            {
                entity.HasOne(d => d.User)
                    .WithMany(p => p.Notification)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("FK_53C0B3DC");
            });

            modelBuilder.Entity<QrtzBlobTriggers>(entity =>
            {
                entity.HasKey(e => new { e.SchedName, e.TriggerName, e.TriggerGroup });
            });

            modelBuilder.Entity<QrtzCalendars>(entity =>
            {
                entity.HasKey(e => new { e.SchedName, e.CalendarName });
            });

            modelBuilder.Entity<QrtzCronTriggers>(entity =>
            {
                entity.HasKey(e => new { e.SchedName, e.TriggerName, e.TriggerGroup });

                entity.HasOne(d => d.QrtzTriggers)
                    .WithOne(p => p.QrtzCronTriggers)
                    .HasForeignKey<QrtzCronTriggers>(d => new { d.SchedName, d.TriggerName, d.TriggerGroup })
                    .HasConstraintName("FK_QRTZ_CRON_TRIGGERS_QRTZ_TRIGGERS");
            });

            modelBuilder.Entity<QrtzFiredTriggers>(entity =>
            {
                entity.HasKey(e => new { e.SchedName, e.EntryId });
            });

            modelBuilder.Entity<QrtzJobDetails>(entity =>
            {
                entity.HasKey(e => new { e.SchedName, e.JobName, e.JobGroup });
            });

            modelBuilder.Entity<QrtzLocks>(entity =>
            {
                entity.HasKey(e => new { e.SchedName, e.LockName });
            });

            modelBuilder.Entity<QrtzPausedTriggerGrps>(entity =>
            {
                entity.HasKey(e => new { e.SchedName, e.TriggerGroup });
            });

            modelBuilder.Entity<QrtzSchedulerState>(entity =>
            {
                entity.HasKey(e => new { e.SchedName, e.InstanceName });
            });

            modelBuilder.Entity<QrtzSimpleTriggers>(entity =>
            {
                entity.HasKey(e => new { e.SchedName, e.TriggerName, e.TriggerGroup });

                entity.HasOne(d => d.QrtzTriggers)
                    .WithOne(p => p.QrtzSimpleTriggers)
                    .HasForeignKey<QrtzSimpleTriggers>(d => new { d.SchedName, d.TriggerName, d.TriggerGroup })
                    .HasConstraintName("FK_QRTZ_SIMPLE_TRIGGERS_QRTZ_TRIGGERS");
            });

            modelBuilder.Entity<QrtzSimpropTriggers>(entity =>
            {
                entity.HasKey(e => new { e.SchedName, e.TriggerName, e.TriggerGroup });

                entity.HasOne(d => d.QrtzTriggers)
                    .WithOne(p => p.QrtzSimpropTriggers)
                    .HasForeignKey<QrtzSimpropTriggers>(d => new { d.SchedName, d.TriggerName, d.TriggerGroup })
                    .HasConstraintName("FK_QRTZ_SIMPROP_TRIGGERS_QRTZ_TRIGGERS");
            });

            modelBuilder.Entity<QrtzTriggers>(entity =>
            {
                entity.HasKey(e => new { e.SchedName, e.TriggerName, e.TriggerGroup });

                entity.HasOne(d => d.QrtzJobDetails)
                    .WithMany(p => p.QrtzTriggers)
                    .HasForeignKey(d => new { d.SchedName, d.JobName, d.JobGroup })
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_QRTZ_TRIGGERS_QRTZ_JOB_DETAILS");
            });

            modelBuilder.Entity<Sponsor>(entity =>
            {
                entity.HasOne(d => d.Benefit)
                    .WithMany(p => p.Sponsor)
                    .HasForeignKey(d => d.BenefitId)
                    .HasConstraintName("FK_A324256A");
            });

            modelBuilder.Entity<Subscription>(entity =>
            {
                entity.HasOne(d => d.Transaction)
                    .WithMany(p => p.Subscription)
                    .HasForeignKey(d => d.TransactionId)
                    .HasConstraintName("FK_A7770EA7");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Subscription)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("FK_1D720D43");
            });

            modelBuilder.Entity<Transaction>(entity =>
            {
                entity.HasOne(d => d.SubscriptionNavigation)
                    .WithMany(p => p.TransactionNavigation)
                    .HasForeignKey(d => d.SubscriptionId)
                    .HasConstraintName("FK_A746D101");
            });

            modelBuilder.Entity<UserReview>(entity =>
            {
                entity.HasOne(d => d.Job)
                    .WithMany(p => p.UserReview)
                    .HasForeignKey(d => d.JobId)
                    .HasConstraintName("FK_B9591073");

                entity.HasOne(d => d.Reviewer)
                    .WithMany(p => p.UserReviewReviewer)
                    .HasForeignKey(d => d.ReviewerId)
                    .HasConstraintName("FK_5E886501");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.UserReviewUser)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("FK_70650A8E");
            });

            modelBuilder.Entity<UserToken>(entity =>
            {
                entity.HasOne(d => d.User)
                    .WithMany(p => p.UserToken)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("FK_87DEA058");
            });

            modelBuilder.Entity<UserTrade>(entity =>
            {
                entity.HasOne(d => d.Trade)
                    .WithMany(p => p.UserTrade)
                    .HasForeignKey(d => d.TradeId)
                    .HasConstraintName("FK_D1B8135C");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.UserTrade)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("FK_4645B3A5");
            });

            OnModelCreatingGeneratedProcedures(modelBuilder);
            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}