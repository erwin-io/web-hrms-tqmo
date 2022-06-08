using HRMS.Data.Core;
using HRMS.Data.Interface;
using HRMS.Data.Entity;
using System.Collections.Generic;
using System.Data;
using System;
using Dapper;
using System.Linq;

namespace HRMS.Data
{
    public class LookupTableDAC : RepositoryBase<LookupTableModel>, ILookupTableRepositoryDAC
    {
        private readonly IDbConnection _dBConnection;

        #region CONSTRUCTORS
        public LookupTableDAC(IDbConnection dbConnection)
        {
            _dBConnection = dbConnection ?? throw new ArgumentNullException(nameof(dbConnection));
        }
        #endregion

        public override string Add(LookupTableModel model) => throw new NotImplementedException();

        public override LookupTableModel Find(string id) => throw new NotImplementedException();

        public override List<LookupTableModel> GetAll() => throw new NotImplementedException();

        public override bool Remove(string id) => throw new NotImplementedException();
        public override bool Update(LookupTableModel model) => throw new NotImplementedException();

        public List<LookupTableModel> FindLookupByTableNames(string TableNames)
        {
            var results = new List<LookupTableModel>();
            try
            {
                var lookup = new Dictionary<string, LookupTableModel>();

                _dBConnection.Query("usp_lookuptable_getByTableNames",
                new[]
                {
                    typeof(LookupTableModel),
                    typeof(LookupModel),
                }, obj =>
                {
                    LookupTableModel lt = obj[0] as LookupTableModel;
                    LookupModel l = obj[1] as LookupModel;
                    LookupTableModel model;
                    if (!lookup.TryGetValue(lt.LookupName, out model))
                        lookup.Add(lt.LookupName, model = lt);
                    if (model.LookupData == null)
                        model.LookupData = new List<LookupModel>();
                    model.LookupData.Add(l);
                    return model;
                },
                new
                {
                    TableNames = TableNames,
                }, splitOn: "LookupName,Id", commandType: CommandType.StoredProcedure).ToList();
                if (lookup.Values.Any())
                {
                    results.AddRange(lookup.Values);
                }
                return results;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<LookupTableModel> FindEnforcementUnitByEnforcementStationId(string EnforcementStationId)
        {
            var results = new List<LookupTableModel>();
            try
            {
                var lookup = new Dictionary<string, LookupTableModel>();

                _dBConnection.Query("usp_enforcementunit_getLookupByEnforcementStationId",
                new[]
                {
                    typeof(LookupTableModel),
                    typeof(LookupModel),
                }, obj =>
                {
                    LookupTableModel lt = obj[0] as LookupTableModel;
                    LookupModel l = obj[1] as LookupModel;
                    LookupTableModel model;
                    if (!lookup.TryGetValue(lt.LookupName, out model))
                        lookup.Add(lt.LookupName, model = lt);
                    if (model.LookupData == null)
                        model.LookupData = new List<LookupModel>();
                    model.LookupData.Add(l);
                    return model;
                },
                new
                {
                    EnforcementStationId = EnforcementStationId,
                }, splitOn: "LookupName,Id", commandType: CommandType.StoredProcedure).ToList();
                if (lookup.Values.Any())
                {
                    results.AddRange(lookup.Values);
                }
                return results;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
