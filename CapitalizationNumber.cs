using System;
using System.Text;

namespace Models
{
	/// <summary>
	/// 小写数字转换成大写管理类
	/// </summary>
	public static class CapitalizationNumber
	{

		//string s = double.Parse(this.TextBox1.Text).ToString("#L#E#D#C#K#E#D#C#J#E#D#C#I#E#D#C#H#E#D#C#G#E#D#C#F#E#D#C#.0B0A");//d + "\n" +
		//string d = Regex.Replace(s, @"((?<=-|^)[^1-9]*)|((?'z'0)[0A-E]*((?=[1-9])|(?'-z'(?=[F-L\.]|$))))|((?'b'[F-L])(?'z'0)[0A-L]*((?=[1-9])|(?'-z'(?=[\.]|$))))", "${b}${z}");
		//Response.Write( Regex.Replace(d, ".", delegate(Match m) { return "负元空零壹贰叁肆伍陆柒捌玖空空空空空空空分角拾佰仟萬億兆京垓秭穰"[m.Value[0] - '-'].ToString(); }));

		private static readonly string[] CapArray = new string[] { "零", "壹", "贰", "叁", "肆", "伍", "陆", "柒", "捌", "玖" };
		private static readonly string[] UnitArray = new string[] { "圆", "拾", "佰", "仟", "万", "拾", "佰", "仟", "亿" };
		private static readonly string[] UnitArray1 = new string[] { "", "拾", "佰", "仟" };
		/// <summary>
		/// 创建小写数字转换成大写管理类实例
		/// </summary>
		static CapitalizationNumber() { }

		/// <summary>
		/// 转换四位金额
		/// </summary>
		/// <param name="moneyBuilder">保存转换后大写金额</param>
		/// <param name="fourMoney">小写金额</param>
		/// <param name="removeStartZero">初始位零是否需要移除</param>
		private static void ChangeFourMoney(ref StringBuilder moneyBuilder, int fourMoney, bool removeStartZero)
		{
			StringBuilder tempBuilder = new StringBuilder(50);
			int tempDe = 0; int tempMoney = fourMoney;
			for (int powValue = 3; powValue >= 0; powValue--)
			{
				tempDe = (int)Math.Pow(10D, powValue);
				if (tempDe <= tempMoney)
				{
					int unit = (int)(tempMoney / tempDe);
					tempBuilder.AppendFormat("{0}{1}", CapArray[unit], UnitArray1[powValue]);
					tempMoney = tempMoney - unit * tempDe;
				}
				else
				{
					tempBuilder.Append("零");
				}
			}
			tempBuilder.Replace("零零零", "零");
			tempBuilder.Replace("零零", "零");
			if (removeStartZero && tempBuilder[0] == '零')
				tempBuilder.Remove(0, 1);
			if (tempBuilder[tempBuilder.Length - 1] == '零')
				tempBuilder.Remove(tempBuilder.Length - 1, 1);
			moneyBuilder.Append(tempBuilder.ToString());
		}

		/// <summary>
		/// 将小写数字转换为大写数字
		/// </summary>
		/// <param name="money">小写的数字金额</param>
		/// <returns>转换后的数字写法</returns>
		public static string StartChange(decimal money, bool upperCase)
		{
			if (!upperCase)
			{
				return string.Format("{0:C2}", money);
			}
			decimal tempMoney = money;
			if (tempMoney == 0)
				return @"¥零圆整";
			else if (tempMoney < 0)
				return string.Empty;
			StringBuilder moneyBuilder = new StringBuilder(@"¥", 50);
			//输出亿
			int deYiMoney = (int)(money / 100000000);
			tempMoney = tempMoney - deYiMoney * 100000000;
			if (deYiMoney > 0)
			{
				ChangeFourMoney(ref moneyBuilder, deYiMoney, true);
				moneyBuilder.Append("亿");
			}
			//输出万
			int deWanMoney = (int)(tempMoney / 10000);
			tempMoney = tempMoney - deWanMoney * 10000;
			if (deWanMoney > 0)
			{
				ChangeFourMoney(ref moneyBuilder, deWanMoney, deYiMoney <= 0);
				moneyBuilder.Append("万");
			}
			//输出个位
			int deGeMoney = (int)tempMoney;
			tempMoney = tempMoney - deGeMoney;
			if (deGeMoney > 0)
			{
				ChangeFourMoney(ref moneyBuilder, deGeMoney, deYiMoney <= 0 && deWanMoney <= 0);
			}

			#region 转换小数位
			if (tempMoney > 0)
			{
				if (money - tempMoney > 0)
					moneyBuilder.AppendFormat("圆");
				if (tempMoney >= 0.1M)
				{
					int unit = (int)(tempMoney / 0.1M);
					if (unit > 0)
						moneyBuilder.AppendFormat("{0}角", CapArray[unit]);
					else
						moneyBuilder.Append("角");

					tempMoney = tempMoney - unit * 0.1M;
					if (tempMoney >= 0.01M)
					{
						unit = (int)(tempMoney / 0.01M);
						moneyBuilder.AppendFormat("{0}分", CapArray[unit]);
					}
					else
					{
						moneyBuilder.AppendFormat("整");
					}
				}
				else
				{
					if (tempMoney >= 0.01M)
					{
						int unit = (int)(tempMoney / 0.01M);
						if (money >= 1)
							moneyBuilder.AppendFormat("零{0}分", CapArray[unit]);
						else
							moneyBuilder.AppendFormat("{0}分", CapArray[unit]);
					}
				}
			}
			else
			{
				moneyBuilder.AppendFormat("圆整");
			}
			#endregion

			return moneyBuilder.ToString();
		}

		/// <summary>
		/// 按位转换数字大小写
		/// </summary>
		/// <param name="money">需要转换的金额</param>
		/// <param name="positionNumber">需要转换的位数</param>
		/// <param name="upperCase">转换后需要输出大写还是小写,true为大写，false为小写。</param>
		/// <param name="defaultString">如果当前位置数字为零则输出的默认字符，为空则输出零</param>
		/// <returns></returns>
		public static string StartChange(decimal money, decimal positionNumber, bool upperCase, string defaultString)
		{
			int tempInt = (int)(money / positionNumber);
			if (tempInt > 0)
			{
				if (tempInt >= 10)
				{
					int tempResult = (int)(tempInt / 10);
					tempInt = tempInt - tempResult * 10;
				}
				if (upperCase)  //输出大写
				{
					return CapArray[tempInt];
				}
				return Convert.ToString(tempInt);
			}
			else
			{
				if (string.IsNullOrEmpty(defaultString))
				{
					if (upperCase)  //输出大写
					{
						return CapArray[0];
					}
					return "0";
				}
			}
			return defaultString;
		}
	}
}
