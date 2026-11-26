using System.Data;

namespace AIOSICT011.DataAccess
{
    // Small tables used to fill combo boxes
    public static class LookupData
    {
        public static DataTable GetServiceTypes(bool includeAll)
        {
            DataTable dt = Db.GetData("SELECT ServiceTypeID, TypeName FROM ServiceTypes ORDER BY TypeName");
            if (includeAll)
            {
                DataRow row = dt.NewRow();
                row["ServiceTypeID"] = 0;
                row["TypeName"] = "-- All --";
                dt.Rows.InsertAt(row, 0);
            }
            return dt;
        }

        public static DataTable GetRegions(bool includeAll)
        {
            DataTable dt = Db.GetData(
                "SELECT RegionID, Unit + ' / ' + Ward + ' / ' + Block AS RegionName FROM Regions ORDER BY Unit");
            if (includeAll)
            {
                DataRow row = dt.NewRow();
                row["RegionID"] = 0;
                row["RegionName"] = "-- All --";
                dt.Rows.InsertAt(row, 0);
            }
            return dt;
        }
    }
}
