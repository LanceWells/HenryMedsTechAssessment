namespace HenryMeds.Util;

public static class DateTimeUtils
{
  /// <summary>
  /// 
  /// </summary>
  /// <param name="dateTime"></param>
  /// <param name="timeSpan"></param>
  /// <returns></returns>
  /// <seealso cref="https://stackoverflow.com/a/7029464/17503966"/>
  public static DateTime RoundDateTime(DateTime dateTime, TimeSpan timeSpan)
  {
    return new DateTime((dateTime.Ticks + timeSpan.Ticks - 1) / timeSpan.Ticks * timeSpan.Ticks, dateTime.Kind);
  }

  public static DateTime Round15Mintues(DateTime dateTime)
  {
    return RoundDateTime(dateTime, TimeSpan.FromMinutes(15));
  }
}

