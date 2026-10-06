// Process scheduling and event handling: timers, global events, and cron-based scheduling configuration
using System;
using System.Collections.Generic;

namespace OutSystems.Model.Processes;

public enum EntityActionKind {
    None,
    Create,
    Update,
}

// Schedule item for specific days of the month (1-31)
public interface IDayOfMonth : IScheduleItem {
    ICollection<int> DayOfMonthValues { get; }
}

// Schedule item for specific days of the week (Sunday-Saturday)
public interface IDayOfWeek : IScheduleItem {
    ICollection<int> DayOfWeekValues { get; }
}

// Application-wide event that can be raised and handled across the module
public interface IGlobalEvent : IGlobalEventSignature, OutSystems.Model.IShareableESpaceObject<IGlobalEvent, IGlobalEventSignature> {
    new string CreatedBy { get; set; }
    OutSystems.Model.Enumerations.CreatedByTool CreatedByTool { get; }
    new string Description { get; set; }
    new OutSystems.Model.IFolder Folder { get; set; }
    new string LastModifiedBy { get; set; }
    OutSystems.Model.Enumerations.CreatedByTool LastModifiedByTool { get; }
    new DateTime LastModifiedDate { get; set; }
    OutSystems.Model.Enumerations.CreatedByTool ModifiedByTools { get; }
    bool Public { get; set; }
    new OutSystems.Model.ISequence<OutSystems.Model.IInputParameter> InputParameters { get; }
    OutSystems.Model.IInputParameter CreateInputParameter(string name = null, OutSystems.Model.IKey key = null);
}

// Handler that responds to a global event by executing a server action
public interface IGlobalEventHandler : OutSystems.Model.IObject {
    IGlobalEventSignature Event { get; }
    OutSystems.Model.Logic.IServerAction Handler { get; set; }
    string Name { get; set; }
    IEnumerable<OutSystems.Model.IArgument> Arguments { get; }
}

public interface IGlobalEventSignature : OutSystems.Model.IObjectSignature {
    string CreatedBy { get; }
    string Description { get; }
    OutSystems.Model.IFolderSignature Folder { get; }
    string LastMergedBy { get; }
    DateTime LastMergedDate { get; }
    string LastModifiedBy { get; }
    DateTime LastModifiedDate { get; }
    string Name { get; set; }
    IEnumerable<OutSystems.Model.IInputParameterSignature> InputParameters { get; }
    IEnumerable<IGlobalEventHandler> Handlers { get; }
    IGlobalEventHandler CreateHandler(OutSystems.Model.IKey key = null);
}

// Schedule item for specific hours of the day (0-23)
public interface IHour : IScheduleItem {
    ICollection<int> HourValues { get; }
}

// Schedule item for specific minutes of the hour (0-59)
public interface IMinute : IScheduleItem {
    ICollection<int> MinuteValues { get; }
}

// Schedule item for specific months of the year (1-12)
public interface IMonth : IScheduleItem {
    ICollection<int> MonthValues { get; }
}

// Cron-style schedule configuration for timer execution
public interface ISchedule : OutSystems.Model.IObject {
    bool WhenPublished { get; set; }
    OutSystems.Model.ISequence<IScheduleItem> ScheduleItems { get; }
    IDayOfMonth CreateDayOfMonth(OutSystems.Model.IKey key = null);
    IDayOfWeek CreateDayOfWeek(OutSystems.Model.IKey key = null);
    IHour CreateHour(OutSystems.Model.IKey key = null);
    IMinute CreateMinute(OutSystems.Model.IKey key = null);
    IMonth CreateMonth(OutSystems.Model.IKey key = null);
    ITimeOfDay CreateTimeOfDay(OutSystems.Model.IKey key = null);
    IWeekOfMonth CreateWeekOfMonth(OutSystems.Model.IKey key = null);
    string ToCronExpression();
    string ToLegacyExpression();
    void FromLegacyExpression(string legacyExpression);
    void FromCronExpression(string cronExpression);
    void Reset();
    ISchedule Any<TScheduleItem>() where TScheduleItem: IScheduleItem;
    ISchedule AtOrOn<TScheduleItem>(int[] values) where TScheduleItem: IScheduleItem;
    ISchedule AtOrOn(DayOfWeek[] values);
    ISchedule AtOrOn(int hour, int minute);
    ISchedule Every<TScheduleItem>(int[] values) where TScheduleItem: IScheduleItem;
    ISchedule Every(DayOfWeek[] values);
    ISchedule Between<TScheduleItem>(int start, int end) where TScheduleItem: IScheduleItem;
    ISchedule Between(DayOfWeek start, DayOfWeek end);
}

// Base interface for schedule components (hour, minute, day, etc.)
public interface IScheduleItem : OutSystems.Model.IObject {
    OutSystems.Model.Enumerations.ScheduleInterval Interval { get; set; }
}

// Schedule item for a specific time of day (hour and minute)
public interface ITimeOfDay : IScheduleItem {
    int Hour { get; }
    int Minute { get; }
}

// Scheduled background job that executes an action at specified intervals
public interface ITimer : OutSystems.Model.IObject {
    OutSystems.Model.Logic.IActionSignature Action { get; set; }
    string CreatedBy { get; set; }
    string Description { get; set; }
    OutSystems.Model.IFolder Folder { get; set; }
    OutSystems.Model.Enumerations.BooleanWithInheritance IsMultiTenant { get; set; }
    string LastMergedBy { get; }
    DateTime LastMergedDate { get; }
    string LastModifiedBy { get; set; }
    DateTime LastModifiedDate { get; set; }
    string Name { get; set; }
    OutSystems.Model.Enumerations.Priority Priority { get; set; }
    string Schedule { get; set; }
    Nullable<int> TimeoutInMinutes { get; set; }
    IEnumerable<OutSystems.Model.IArgument> Arguments { get; }
    ISchedule ScheduleConfiguration { get; }
    OutSystems.Model.Logic.IActionSignature WakeTimerAction { get; }
    string CronSchedule { get; set; }
}

// Schedule item for specific weeks of the month (1-5)
public interface IWeekOfMonth : IScheduleItem {
    ICollection<int> WeekOfMonthValues { get; }
}

