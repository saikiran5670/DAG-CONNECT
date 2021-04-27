﻿using net.atos.daf.ct2.poigeofence.entity;
using net.atos.daf.ct2.poigeofence.repository;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace net.atos.daf.ct2.poigeofence
{
    public class GeofenceManager: IGeofenceManager
    {
        private readonly IGeofenceRepository geofenceRepository;

        public GeofenceManager(IGeofenceRepository _geofenceRepository)
        {
            geofenceRepository = _geofenceRepository;
        }
        public async Task<bool> DeleteGeofence(List<int> geofenceIds, int organizationID)
        {
            return await geofenceRepository.DeleteGeofence(geofenceIds, organizationID);
        }
        public async Task<Geofence> CreatePolygonGeofence(Geofence geofence)
        {
            return await geofenceRepository.CreatePolygonGeofence(geofence);
        }
        public async Task<IEnumerable<GeofenceEntityResponce>> GetAllGeofence(GeofenceEntityRequest geofenceEntityRequest)
        {
            return await geofenceRepository.GetAllGeofence(geofenceEntityRequest);
        }
        public async Task<List<Geofence>> CreateCircularGeofence(List<Geofence> geofence)
        {
            return await geofenceRepository.CreateCircularGeofence(geofence);
        }
        public async Task<Geofence> UpdatePolygonGeofence(Geofence geofence)
        {
            return await geofenceRepository.UpdatePolygonGeofence(geofence);
        }
        public async Task<IEnumerable<Geofence>> GetGeofenceByGeofenceID(int organizationId, int geofenceId)
        {
            return await geofenceRepository.GetGeofenceByGeofenceID(organizationId, geofenceId);
        }
        public async Task<List<Geofence>> BulkImportGeofence(List<Geofence> geofences)
        {
            return await geofenceRepository.BulkImportGeofence(geofences);
        }
    }
}
