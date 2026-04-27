using Coucher.Shared.Models.DAL.Admin;
using Coucher.Shared.Models.DAL.Exercises;
using Coucher.Shared.Models.DAL.Notifications;
using Coucher.Shared.Models.DAL.Tasks;
using Coucher.Shared.Models.DAL.Users;
using Coucher.Shared.Models.Enums;
using Coucher.Shared.Models.Internal.Mocking;
using Coucher.Shared;

namespace Coucher.Lib.Mocking;

internal sealed class MockDataGenerator
{
    public MockSeedData Generate(MockSeedOptions options)
    {
        var userCount = Clamp(options.UserCount, 12, 1, 200);
        var exerciseCount = Clamp(options.ExerciseCount, 3, 1, 50);
        var additionalParticipantsPerExercise = Clamp(options.AdditionalParticipantsPerExercise, 4, 0, 50);
        var taskTemplateCount = Clamp(options.TaskTemplateCount, 12, 1, 200);
        var tasksPerExercise = Clamp(options.TasksPerExercise, 20, 1, 500);
        var notificationsPerUser = Clamp(options.NotificationsPerUser, 2, 0, 100);

        var now = DateTime.UtcNow;
        var rng = new Random();

        var closedListItems = BuildClosedListItems(now);

        var byKey = closedListItems
            .GroupBy(x => x.Key, StringComparer.OrdinalIgnoreCase)
            .ToDictionary(g => g.Key, g => g.ToList(), StringComparer.OrdinalIgnoreCase);

        var units = BuildUnits(now, byKey[ConstantValues.UnitEchelonClosedListKey]);
        var users = BuildUsers(now, userCount, units, rng);
        var userRoles = BuildUserRoles(now, users);

        var exercises = BuildExercises(now, exerciseCount, units, byKey[ConstantValues.ExerciseStatusClosedListKey], rng);
        var exerciseParticipants = BuildExerciseParticipants(
            now,
            exercises,
            users,
            additionalParticipantsPerExercise,
            rng
        );
        var unitContacts = BuildExerciseUnitContacts(now, exercises, rng);
        var exerciseInfluencers = BuildExerciseInfluencers(now, exercises, byKey[ConstantValues.InfluencerClosedListKey], rng);
        var exerciseSections = BuildExerciseSections(now, exercises, byKey[ConstantValues.SectionClosedListKey], rng);

        var templates = BuildTaskTemplates(
            now,
            taskTemplateCount,
            byKey[ConstantValues.TaskSeriesClosedListKey],
            byKey[ConstantValues.TaskCategoryClosedListKey],
            rng
        );
        var templateInfluencers = BuildTaskTemplateInfluencers(now, templates, byKey[ConstantValues.InfluencerClosedListKey], rng);
        var templateDependencies = BuildTaskTemplateDependencies(now, templates, rng);

        var tasks = BuildExerciseTasks(
            now,
            exercises,
            templates,
            byKey[ConstantValues.TaskStatusClosedListKey],
            byKey[ConstantValues.TaskSeriesClosedListKey],
            byKey[ConstantValues.TaskCategoryClosedListKey],
            tasksPerExercise,
            rng
        );
        var responsibleLinks = BuildResponsibleUsers(now, tasks, users, rng);
        var taskDependencies = BuildTaskDependencies(now, tasks, rng);
        var notifications = BuildUserNotifications(now, users, exercises, tasks, notificationsPerUser, rng);

        var data = new MockSeedData
        {
            ClosedListItems = closedListItems,
            Units = units,
            Users = users,
            UserRoles = userRoles,
            Exercises = exercises,
            ExerciseParticipants = exerciseParticipants,
            ExerciseUnitContacts = unitContacts,
            ExerciseInfluencers = exerciseInfluencers,
            ExerciseSections = exerciseSections,
            TaskTemplates = templates,
            TaskTemplateDependencies = templateDependencies,
            TaskTemplateInfluencers = templateInfluencers,
            ExerciseTasks = tasks,
            ExerciseTaskResponsibleUsers = responsibleLinks,
            TaskDependencies = taskDependencies,
            UserNotifications = notifications
        };

        return data;
    }

    private static int Clamp(int? value, int defaultValue, int min, int max)
    {
        var v = value ?? defaultValue;
        if (v < min) return min;
        if (v > max) return max;
        return v;
    }

    private static List<ClosedListItem> BuildClosedListItems(DateTime now)
    {
        var items = new List<ClosedListItem>();

        AddList(items, ConstantValues.ExerciseStatusClosedListKey, new[] { "טיוטה", "פעיל", "בארכיון" }, now);
        AddList(items, ConstantValues.TaskStatusClosedListKey, new[] { "לא התחיל", "בתהליך", "בוצע" }, now);
        AddList(items, ConstantValues.TaskSeriesClosedListKey, new[] { "סדרה א", "סדרה ב", "סדרה ג" }, now);
        AddList(items, ConstantValues.TaskCategoryClosedListKey, new[] { "תכנון", "הכנה", "ביצוע", "בקרה" }, now);
        AddList(items, ConstantValues.SectionClosedListKey, new[] { "מבצעים", "מודיעין", "לוגיסטיקה", "תקשוב" }, now);
        AddList(items, ConstantValues.InfluencerClosedListKey, new[] { "אוכלוסיה", "תשתיות", "מידע", "תשתיות לאומיות", "בריאות" }, now);
        AddList(items, ConstantValues.UnitEchelonClosedListKey, new[] { "גדוד", "חטיבה", "אוגדה", "פיקוד" }, now);

        return items;
    }

    private static void AddList(List<ClosedListItem> into, string key, IEnumerable<string> values, DateTime now)
    {
        var i = 0;
        foreach (var value in values)
        {
            into.Add(new ClosedListItem
            {
                Id = Guid.NewGuid(),
                Key = key,
                Value = value,
                Description = null,
                DisplayOrder = i++,
                CreationTime = now,
                LastUpdateTime = now
            });
        }
    }

    private static List<Unit> BuildUnits(DateTime now, List<ClosedListItem> echelons)
    {
        var units = new List<Unit>
        {
            new()
            {
                Id = Guid.NewGuid(),
                Name = "חטיבה 7",
                EchelonId = echelons[1].Id,
                CreationTime = now,
                LastUpdateTime = now
            },
            new()
            {
                Id = Guid.NewGuid(),
                Name = "גדוד 13",
                EchelonId = echelons[0].Id,
                CreationTime = now,
                LastUpdateTime = now
            },
            new()
            {
                Id = Guid.NewGuid(),
                Name = "אוגדה 36",
                EchelonId = echelons[2].Id,
                CreationTime = now,
                LastUpdateTime = now
            },
            new()
            {
                Id = Guid.NewGuid(),
                Name = "פיקוד הדרום",
                EchelonId = echelons[3].Id,
                CreationTime = now,
                LastUpdateTime = now
            }
        };

        return units;
    }

    private static List<UserProfile> BuildUsers(DateTime now, int count, List<Unit> units, Random rng)
    {
        var firstNames = new[]
        {
            "אורי", "נועה", "יואב", "תמר", "אלון", "מיכל", "דניאל", "שירה", "עומר", "יעל", "רון", "מאיה"
        };
        var lastNames = new[]
        {
            "כהן", "לוי", "מזרחי", "פרץ", "ביטון", "אברהם", "דויד", "ממן", "שלום", "חדד", "סויסה", "נחום"
        };
        var ranks = new[] { "סרן", "רב סרן", "סגן", "רס\"ן", "אל\"מ" };
        var positions = new[] { "מפקד צוות", "קצין מבצעים", "קצין מודיעין", "קצין לוגיסטיקה", "קצין תקשוב" };

        var users = new List<UserProfile>(capacity: count);
        var baseIdentity = 300000000;

        for (var i = 0; i < count; i++)
        {
            var first = firstNames[i % firstNames.Length];
            var last = lastNames[(i * 3) % lastNames.Length];
            var unit = units[rng.Next(units.Count)];

            users.Add(new UserProfile
            {
                Id = Guid.NewGuid(),
                IdentityNumber = (baseIdentity + i).ToString(),
                FirstName = first,
                LastName = last,
                PersonalNumber = $"PN-{1000 + i}",
                ExternalId = null,
                UnitId = unit.Id,
                Rank = ranks[rng.Next(ranks.Length)],
                Position = positions[rng.Next(positions.Length)],
                PhoneNumber = $"050-{rng.Next(1000000, 9999999)}",
                CivilianEmail = $"user{i + 1}@example.test",
                MilitaryEmail = $"user{i + 1}@idf.test",
                ProfileImageUrl = $"https://example.test/images/profile/{i + 1}.png",
                PasswordHash = null,
                LastLoginTime = null,
                CreationTime = now,
                LastUpdateTime = now,
                Roles = new List<UserRole>(),
                ExerciseParticipants = new List<ExerciseParticipant>(),
                ResponsibleTaskLinks = new List<ExerciseTaskResponsibleUser>(),
                Notifications = new List<UserNotification>()
            });
        }

        return users;
    }

    private static List<UserRole> BuildUserRoles(DateTime now, List<UserProfile> users)
    {
        var roles = new List<UserRole>();
        var adminUser = users.First();

        roles.Add(new UserRole
        {
            Id = Guid.NewGuid(),
            UserId = adminUser.Id,
            Role = GlobalRole.Admin,
            IsActive = true,
            AssignedTime = now,
            AssignedByUserId = adminUser.Id
        });

        foreach (var user in users.Skip(1))
        {
            roles.Add(new UserRole
            {
                Id = Guid.NewGuid(),
                UserId = user.Id,
                Role = GlobalRole.User,
                IsActive = true,
                AssignedTime = now,
                AssignedByUserId = adminUser.Id
            });
        }

        return roles;
    }

    private static List<Exercise> BuildExercises(
        DateTime now,
        int count,
        List<Unit> units,
        List<ClosedListItem> exerciseStatuses,
        Random rng
    )
    {
        var names = new[]
        {
            "תרגיל חירום מחוזי",
            "תרגיל מוכנות חטיבתי",
            "תרגיל מטה רב-זרועי",
            "תרגיל הגנה מרחבית"
        };

        var activeStatus = exerciseStatuses.First(x => x.Value == "פעיל");

        var exercises = new List<Exercise>(capacity: count);
        for (var i = 0; i < count; i++)
        {
            var start = DateOnly.FromDateTime(now.Date.AddDays(-14 + (i * 7)));
            var end = DateOnly.FromDateTime(now.Date.AddDays(14 + (i * 10)));
            var traineeUnit = units[rng.Next(units.Count)];
            var trainerUnit = units[rng.Next(units.Count)];

            exercises.Add(new Exercise
            {
                Id = Guid.NewGuid(),
                Name = $"{names[i % names.Length]} #{i + 1}",
                Description = "תרגיל לדוגמה שנוצר אוטומטית לצורך בדיקות.",
                StartDate = start,
                EndDate = end,
                TraineeUnitId = traineeUnit.Id,
                TrainerUnitId = trainerUnit.Id,
                StatusId = activeStatus.Id,
                CompressionFactor = 1.0,
                CreationTime = now,
                LastUpdateTime = now,
                CompletionTime = null,
                ArchiveTime = null,
                ArchivedByUserId = null,
                Participants = new List<ExerciseParticipant>(),
                UnitContacts = new List<ExerciseUnitContact>(),
                Influencers = new List<ExerciseInfluencer>(),
                Sections = new List<ExerciseSection>(),
                Tasks = new List<ExerciseTask>()
            });
        }

        return exercises;
    }

    private static List<ExerciseParticipant> BuildExerciseParticipants(
        DateTime now,
        List<Exercise> exercises,
        List<UserProfile> users,
        int additionalParticipantsPerExercise,
        Random rng
    )
    {
        var participants = new List<ExerciseParticipant>();

        foreach (var exercise in exercises)
        {
            var manager = users[rng.Next(users.Count)];
            participants.Add(new ExerciseParticipant
            {
                Id = Guid.NewGuid(),
                ExerciseId = exercise.Id,
                UserId = manager.Id,
                Role = ExerciseRole.ExerciseManager,
                CreationTime = now
            });

            for (var i = 0; i < additionalParticipantsPerExercise; i++)
            {
                var user = users[rng.Next(users.Count)];
                participants.Add(new ExerciseParticipant
                {
                    Id = Guid.NewGuid(),
                    ExerciseId = exercise.Id,
                    UserId = user.Id,
                    Role = ExerciseRole.ExerciseParticipant,
                    CreationTime = now
                });
            }
        }

        return participants;
    }

    private static List<ExerciseUnitContact> BuildExerciseUnitContacts(DateTime now, List<Exercise> exercises, Random rng)
    {
        var firstNames = new[] { "אבי", "רות", "חיים", "ליאור", "איילת", "ארז" };
        var lastNames = new[] { "כהן", "לוי", "פרידמן", "אלקיים", "רז", "גרין" };

        var contacts = new List<ExerciseUnitContact>();

        foreach (var exercise in exercises)
        {
            var traineeCount = rng.Next(1, 3);
            for (var i = 0; i < traineeCount; i++)
            {
                contacts.Add(new ExerciseUnitContact
                {
                    Id = Guid.NewGuid(),
                    ExerciseId = exercise.Id,
                    ContactType = ExerciseUnitContactType.TraineeUnitContact,
                    FirstName = firstNames[rng.Next(firstNames.Length)],
                    LastName = lastNames[rng.Next(lastNames.Length)],
                    PhoneNumber = $"052-{rng.Next(1000000, 9999999)}",
                    ProfileImageUrl = null,
                    CreationTime = now
                });
            }

            var trainerCount = rng.Next(1, 3);
            for (var i = 0; i < trainerCount; i++)
            {
                contacts.Add(new ExerciseUnitContact
                {
                    Id = Guid.NewGuid(),
                    ExerciseId = exercise.Id,
                    ContactType = ExerciseUnitContactType.TrainerUnitContact,
                    FirstName = firstNames[rng.Next(firstNames.Length)],
                    LastName = lastNames[rng.Next(lastNames.Length)],
                    PhoneNumber = $"053-{rng.Next(1000000, 9999999)}",
                    ProfileImageUrl = null,
                    CreationTime = now
                });
            }
        }

        return contacts;
    }

    private static List<ExerciseInfluencer> BuildExerciseInfluencers(
        DateTime now,
        List<Exercise> exercises,
        List<ClosedListItem> influencers,
        Random rng
    )
    {
        var links = new List<ExerciseInfluencer>();

        foreach (var exercise in exercises)
        {
            var selected = influencers.OrderBy(_ => rng.Next()).Take(3).ToList();
            foreach (var influencer in selected)
            {
                links.Add(new ExerciseInfluencer
                {
                    Id = Guid.NewGuid(),
                    ExerciseId = exercise.Id,
                    InfluencerId = influencer.Id
                });
            }
        }

        return links;
    }

    private static List<ExerciseSection> BuildExerciseSections(
        DateTime now,
        List<Exercise> exercises,
        List<ClosedListItem> sections,
        Random rng
    )
    {
        var links = new List<ExerciseSection>();

        foreach (var exercise in exercises)
        {
            var selected = sections.OrderBy(_ => rng.Next()).Take(2).ToList();
            foreach (var section in selected)
            {
                links.Add(new ExerciseSection
                {
                    Id = Guid.NewGuid(),
                    ExerciseId = exercise.Id,
                    SectionId = section.Id
                });
            }
        }

        return links;
    }

    private static List<TaskTemplate> BuildTaskTemplates(
        DateTime now,
        int count,
        List<ClosedListItem> series,
        List<ClosedListItem> categories,
        Random rng
    )
    {
        var names = new[]
        {
            "פתיחת חדר מצב",
            "תיאום עם גורמי חוץ",
            "איסוף נתונים ראשוני",
            "הכנת תכנית עבודה",
            "תדריך פתיחה",
            "בקרת ביצוע",
            "תחקיר ביניים",
            "תחקיר סיום"
        };

        var templates = new List<TaskTemplate>(capacity: count);
        for (var i = 0; i < count; i++)
        {
            templates.Add(new TaskTemplate
            {
                Id = Guid.NewGuid(),
                ParentId = null,
                SeriesId = series[rng.Next(series.Count)].Id,
                CategoryId = categories[rng.Next(categories.Count)].Id,
                SerialNumber = i + 1,
                Name = $"{names[i % names.Length]}",
                Description = "משימת תבנית לדוגמה.",
                Notes = null,
                DefaultWeeksBeforeExerciseStart = rng.Next(1, 12),
                CreationTime = now,
                LastUpdateTime = now,
                Parent = null,
                Series = null,
                Category = null,
                Children = new List<TaskTemplate>(),
                Dependencies = new List<TaskTemplateDependency>(),
                DependedOnBy = new List<TaskTemplateDependency>(),
                Influencers = new List<TaskTemplateInfluencer>()
            });
        }

        return templates;
    }

    private static List<TaskTemplateInfluencer> BuildTaskTemplateInfluencers(
        DateTime now,
        List<TaskTemplate> templates,
        List<ClosedListItem> influencers,
        Random rng
    )
    {
        var links = new List<TaskTemplateInfluencer>();
        foreach (var template in templates)
        {
            var influencer = influencers[rng.Next(influencers.Count)];
            links.Add(new TaskTemplateInfluencer
            {
                Id = Guid.NewGuid(),
                TemplateId = template.Id,
                InfluencerId = influencer.Id,
                CreationTime = now
            });
        }

        return links;
    }

    private static List<TaskTemplateDependency> BuildTaskTemplateDependencies(
        DateTime now,
        List<TaskTemplate> templates,
        Random rng
    )
    {
        var deps = new List<TaskTemplateDependency>();
        var ordered = templates.OrderBy(_ => rng.Next()).ToList();

        for (var i = 1; i < ordered.Count; i++)
        {
            var current = ordered[i];
            var dependsOn = ordered[rng.Next(0, i)];

            deps.Add(new TaskTemplateDependency
            {
                Id = Guid.NewGuid(),
                TemplateId = current.Id,
                DependsOnId = dependsOn.Id,
                CreationTime = now
            });
        }

        return deps;
    }

    private static List<ExerciseTask> BuildExerciseTasks(
        DateTime now,
        List<Exercise> exercises,
        List<TaskTemplate> templates,
        List<ClosedListItem> taskStatuses,
        List<ClosedListItem> series,
        List<ClosedListItem> categories,
        int tasksPerExercise,
        Random rng
    )
    {
        var tasks = new List<ExerciseTask>();
        var defaultStatusId = taskStatuses.First(x => x.Value == "לא התחיל").Id;

        foreach (var exercise in exercises)
        {
            for (var i = 0; i < tasksPerExercise; i++)
            {
                var template = templates[rng.Next(templates.Count)];
                var due = now.AddDays(rng.Next(1, 30));

                tasks.Add(new ExerciseTask
                {
                    Id = Guid.NewGuid(),
                    ExerciseId = exercise.Id,
                    ParentId = null,
                    SourceId = template.Id,
                    SeriesId = series[rng.Next(series.Count)].Id,
                    CategoryId = categories[rng.Next(categories.Count)].Id,
                    StatusId = defaultStatusId,
                    SerialNumber = i + 1,
                    Name = template.Name,
                    Description = "משימה לדוגמה שנגזרה מתבנית.",
                    Notes = null,
                    DueDate = due,
                    LastStatusUpdaterId = null,
                    CreationTime = now,
                    LastUpdateTime = now,
                    LastStatusUpdateTime = null,
                    CompletionTime = null,
                    HasChildren = false,
                    Exercise = null,
                    Parent = null,
                    Source = null,
                    Series = null,
                    Category = null,
                    Status = null,
                    Children = new List<ExerciseTask>(),
                    ResponsibleUsers = new List<ExerciseTaskResponsibleUser>(),
                    Dependencies = new List<TaskDependency>(),
                    DependedOnBy = new List<TaskDependency>()
                });
            }
        }

        return tasks;
    }

    private static List<ExerciseTaskResponsibleUser> BuildResponsibleUsers(
        DateTime now,
        List<ExerciseTask> tasks,
        List<UserProfile> users,
        Random rng
    )
    {
        var links = new List<ExerciseTaskResponsibleUser>();

        foreach (var task in tasks.Where((_, idx) => idx % 3 == 0))
        {
            var user = users[rng.Next(users.Count)];
            links.Add(new ExerciseTaskResponsibleUser
            {
                Id = Guid.NewGuid(),
                TaskId = task.Id,
                UserId = user.Id,
                CreationTime = now
            });
        }

        return links;
    }

    private static List<TaskDependency> BuildTaskDependencies(DateTime now, List<ExerciseTask> tasks, Random rng)
    {
        var deps = new List<TaskDependency>();
        var byExercise = tasks.GroupBy(t => t.ExerciseId).ToList();

        foreach (var group in byExercise)
        {
            var list = group.ToList();
            if (list.Count < 2) continue;

            // Simple chain: task[i] depends on task[i-1]
            for (var i = 1; i < Math.Min(list.Count, 10); i++)
            {
                deps.Add(new TaskDependency
                {
                    Id = Guid.NewGuid(),
                    TaskId = list[i].Id,
                    DependsOnId = list[i - 1].Id,
                    CreationTime = now
                });
            }

            // Add a few random cross-links.
            for (var i = 0; i < 5 && list.Count > 3; i++)
            {
                var a = list[rng.Next(list.Count)];
                var b = list[rng.Next(list.Count)];
                if (a.Id == b.Id) continue;

                deps.Add(new TaskDependency
                {
                    Id = Guid.NewGuid(),
                    TaskId = a.Id,
                    DependsOnId = b.Id,
                    CreationTime = now
                });
            }
        }

        // Ensure uniqueness within this generated set (TaskId, DependsOnId).
        var unique = deps
            .GroupBy(d => new { d.TaskId, d.DependsOnId })
            .Select(g => g.First())
            .ToList();

        return unique;
    }

    private static List<UserNotification> BuildUserNotifications(
        DateTime now,
        List<UserProfile> users,
        List<Exercise> exercises,
        List<ExerciseTask> tasks,
        int notificationsPerUser,
        Random rng
    )
    {
        var titles = new[]
        {
            "משימה חדשה הוקצתה",
            "תזכורת תרגיל מתקרב",
            "משימה עומדת לפוג",
            "עדכון תרגיל"
        };
        var messages = new[]
        {
            "נוספה משימה חדשה ללוח התרגיל.",
            "התרגיל מתקרב. מומלץ לעבור על רשימת המשימות.",
            "משימה מתקרבת לתאריך היעד.",
            "בוצע עדכון בפרטי התרגיל."
        };

        var notifications = new List<UserNotification>();
        for (var u = 0; u < users.Count; u++)
        {
            var user = users[u];
            for (var i = 0; i < notificationsPerUser; i++)
            {
                var exercise = exercises[rng.Next(exercises.Count)];
                var task = tasks[rng.Next(tasks.Count)];

                notifications.Add(new UserNotification
                {
                    Id = Guid.NewGuid(),
                    UserId = user.Id,
                    Type = (NotificationType)rng.Next(0, 6),
                    Severity = (NotificationSeverity)rng.Next(0, 3),
                    Title = titles[rng.Next(titles.Length)],
                    Message = messages[rng.Next(messages.Length)],
                    CreationTime = now.AddMinutes(-rng.Next(0, 60 * 24 * 7)),
                    IsRead = rng.NextDouble() < 0.4,
                    ReadTime = null,
                    ExerciseId = exercise.Id,
                    TaskId = task.Id
                });
            }
        }

        return notifications;
    }
}
