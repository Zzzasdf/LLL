
    public partial class FixedFrequencyManager
    {
        /// 显示格式
        public enum ETimeFormat
        {
            /// 电子格式 00:00:00
            ELEC_HourMinuteSecond = 10001,

            /// 中文格式 00时00分00秒
            CN_HourMinuteSecond = 20001,
        }

        /// 获取显示字符串
        public static string GetTimeFormat(ETimeFormat eTimeFormat, long currentMilliseconds, long endMilliseconds)
        {
            long leftSeconds = (endMilliseconds - currentMilliseconds) / 1000;
            switch (eTimeFormat)
            {
                case ETimeFormat.ELEC_HourMinuteSecond:
                {
                    if (leftSeconds > 0)
                    {
                        long hours = leftSeconds / (60 * 60);
                        long minutes = leftSeconds / 60 % 60;
                        long seconds = leftSeconds % 60;
                        return $"{hours}:{minutes}:{seconds}";
                    }

                    return string.Empty;
                }
                case ETimeFormat.CN_HourMinuteSecond:
                {
                    if (leftSeconds > 0)
                    {
                        long hours = leftSeconds / (60 * 60);
                        long minutes = leftSeconds / 60 % 60;
                        long seconds = leftSeconds % 60;
                        return $"{hours}时{minutes}分{seconds}秒";
                    }

                    return string.Empty;
                }
            }

            return string.Empty;
        }
    }