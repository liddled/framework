using System;

namespace DL.Framework.Common
{
    [Serializable]
	public enum Status
	{
        [TextRep(ETextRepType.Db, "")]
        [TextRep(ETextRepType.Display, "All")]
        [TextRep(ETextRepType.Web, "all")]
        All,

		[TextRep(ETextRepType.Db, "ACTIVE")]
        [TextRep(ETextRepType.Display, "Active")]
        [TextRep(ETextRepType.Web, "active")]
		Active,

		[TextRep(ETextRepType.Db, "INACTIVE")]
        [TextRep(ETextRepType.Display, "Inactive")]
        [TextRep(ETextRepType.Web, "inactive")]
		Inactive
	}

    [Serializable]
    public enum EventAction
    {
        None,
        Add,
        Update,
        Delete
    }

    [Serializable]
    public enum DateView
    {
        [TextRep(ETextRepType.Db, "ALL")]
        [TextRep(ETextRepType.Display, "All")]
        [TextRep(ETextRepType.Web, "all")]
        All,

        [TextRep(ETextRepType.Db, "DAY")]
        [TextRep(ETextRepType.Display, "Day")]
        [TextRep(ETextRepType.Web, "day")]
        Day,

        [TextRep(ETextRepType.Db, "MONTH")]
        [TextRep(ETextRepType.Display, "Month")]
        [TextRep(ETextRepType.Web, "month")]
        Month,

        [TextRep(ETextRepType.Db, "YEAR")]
        [TextRep(ETextRepType.Display, "Year")]
        [TextRep(ETextRepType.Web, "year")]
        Year
    }
}
