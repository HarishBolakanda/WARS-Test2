﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using WARS.IDAL;

namespace WARS.IDAL
{
    public interface ISellerGroupDAL
    {        
        DataSet GetSellerGroupInOutData(string sellerGroupCode, out Int32 iErrorId);
        DataSet AddTerritoryToGroup(string territoryGroupCode, Array territorycode, string userCode,  out Int32 iErrorId);
        DataSet RemoveTerritoryToGroup(string territoryGroupCode, Array territorycode, string userCode,  out Int32 iErrorId);
        DataSet InsertTerritoryGroup(string territoryGroupCode, string territoryGroupName, string userCode, out Int32 iErrorId);
    }
}