import { Component, Input, OnInit } from '@angular/core';

@Component({
  selector: 'app-fleet-overview-filters',
  templateUrl: './fleet-overview-filters.component.html',
  styleUrls: ['./fleet-overview-filters.component.less']
})
export class FleetOverviewFiltersComponent implements OnInit {
@Input() translationData: any;
@Input() detailsData: any;
tabVisibilityStatus: boolean = true;
selectedIndex: number = 0;
filterData =
  {
    "vehicleGroups": [
      {
        "vehicleGroupId": 63,
        "vehicleGroupName": "demo",
        "vehicleId": 29,
        "featureName": "FleetOverview.LogBook",
        "featureKey": "feat_livefleet_logbook",
        "subscribe": true
      },
      {
        "vehicleGroupId": 118,
        "vehicleGroupName": "Test Group",
        "vehicleId": 27,
        "featureName": "FleetOverview.LogBook",
        "featureKey": "feat_livefleet_logbook",
        "subscribe": true
      },
      {
        "vehicleGroupId": 118,
        "vehicleGroupName": "Test Group",
        "vehicleId": 29,
        "featureName": "FleetOverview.LogBook",
        "featureKey": "feat_livefleet_logbook",
        "subscribe": true
      },
      {
        "vehicleGroupId": 63,
        "vehicleGroupName": "demo",
        "vehicleId": 29,
        "featureName": "FleetOverview.HistoryWarning",
        "featureKey": "feat_livefleet_historywarning",
        "subscribe": true
      },
      {
        "vehicleGroupId": 118,
        "vehicleGroupName": "Test Group",
        "vehicleId": 27,
        "featureName": "FleetOverview.HistoryWarning",
        "featureKey": "feat_livefleet_historywarning",
        "subscribe": true
      },
      {
        "vehicleGroupId": 118,
        "vehicleGroupName": "Test Group",
        "vehicleId": 29,
        "featureName": "FleetOverview.HistoryWarning",
        "featureKey": "feat_livefleet_historywarning",
        "subscribe": true
      },
      {
        "vehicleGroupId": 63,
        "vehicleGroupName": "demo",
        "vehicleId": 29,
        "featureName": "FleetOverview.CurrentWarning",
        "featureKey": "feat_livefleet_currentwarning",
        "subscribe": true
      },
      {
        "vehicleGroupId": 118,
        "vehicleGroupName": "Test Group",
        "vehicleId": 27,
        "featureName": "FleetOverview.CurrentWarning",
        "featureKey": "feat_livefleet_currentwarning",
        "subscribe": true
      },
      {
        "vehicleGroupId": 118,
        "vehicleGroupName": "Test Group",
        "vehicleId": 29,
        "featureName": "FleetOverview.CurrentWarning",
        "featureKey": "feat_livefleet_currentwarning",
        "subscribe": true
      },
      {
        "vehicleGroupId": 63,
        "vehicleGroupName": "demo",
        "vehicleId": 29,
        "featureName": "FleetOverview.Status",
        "featureKey": "feat_livefleet_status",
        "subscribe": true
      },
      {
        "vehicleGroupId": 118,
        "vehicleGroupName": "Test Group",
        "vehicleId": 27,
        "featureName": "FleetOverview.Status",
        "featureKey": "feat_livefleet_status",
        "subscribe": true
      },
      {
        "vehicleGroupId": 118,
        "vehicleGroupName": "Test Group",
        "vehicleId": 29,
        "featureName": "FleetOverview.Status",
        "featureKey": "feat_livefleet_status",
        "subscribe": true
      },
      {
        "vehicleGroupId": 63,
        "vehicleGroupName": "demo",
        "vehicleId": 29,
        "featureName": "FleetOverview.NextServiceIn",
        "featureKey": "feat_livefleet_nextservicein",
        "subscribe": true
      },
      {
        "vehicleGroupId": 118,
        "vehicleGroupName": "Test Group",
        "vehicleId": 27,
        "featureName": "FleetOverview.NextServiceIn",
        "featureKey": "feat_livefleet_nextservicein",
        "subscribe": true
      },
      {
        "vehicleGroupId": 118,
        "vehicleGroupName": "Test Group",
        "vehicleId": 29,
        "featureName": "FleetOverview.NextServiceIn",
        "featureKey": "feat_livefleet_nextservicein",
        "subscribe": true
      },
      {
        "vehicleGroupId": 63,
        "vehicleGroupName": "demo",
        "vehicleId": 29,
        "featureName": "FleetOverview.CurrentMileage",
        "featureKey": "feat_livefleet_currentmileage",
        "subscribe": true
      },
      {
        "vehicleGroupId": 118,
        "vehicleGroupName": "Test Group",
        "vehicleId": 27,
        "featureName": "FleetOverview.CurrentMileage",
        "featureKey": "feat_livefleet_currentmileage",
        "subscribe": true
      },
      {
        "vehicleGroupId": 118,
        "vehicleGroupName": "Test Group",
        "vehicleId": 29,
        "featureName": "FleetOverview.CurrentMileage",
        "featureKey": "feat_livefleet_currentmileage",
        "subscribe": true
      },
      {
        "vehicleGroupId": 63,
        "vehicleGroupName": "demo",
        "vehicleId": 29,
        "featureName": "FleetOverview.VehicleHealth",
        "featureKey": "feat_livefleet_vehiclehealth",
        "subscribe": true
      },
      {
        "vehicleGroupId": 118,
        "vehicleGroupName": "Test Group",
        "vehicleId": 27,
        "featureName": "FleetOverview.VehicleHealth",
        "featureKey": "feat_livefleet_vehiclehealth",
        "subscribe": true
      },
      {
        "vehicleGroupId": 118,
        "vehicleGroupName": "Test Group",
        "vehicleId": 29,
        "featureName": "FleetOverview.VehicleHealth",
        "featureKey": "feat_livefleet_vehiclehealth",
        "subscribe": true
      },
      {
        "vehicleGroupId": 63,
        "vehicleGroupName": "demo",
        "vehicleId": 29,
        "featureName": "FleetOverview.ViewAlerts",
        "featureKey": "feat_livefleet_viewalerts",
        "subscribe": true
      },
      {
        "vehicleGroupId": 118,
        "vehicleGroupName": "Test Group",
        "vehicleId": 27,
        "featureName": "FleetOverview.ViewAlerts",
        "featureKey": "feat_livefleet_viewalerts",
        "subscribe": true
      },
      {
        "vehicleGroupId": 118,
        "vehicleGroupName": "Test Group",
        "vehicleId": 29,
        "featureName": "FleetOverview.ViewAlerts",
        "featureKey": "feat_livefleet_viewalerts",
        "subscribe": true
      }
    ],
    "alertLevel": [
      {
        "name": "enumurgencylevel_critical",
        "value": "C"
      },
      {
        "name": "enumurgencylevel_warning",
        "value": "W"
      },
      {
        "name": "enumurgencylevel_advisory",
        "value": "A"
      }
    ],
    "alertCategory": [
      {
        "name": "enumcategory_logisticsalerts",
        "value": "L"
      },
      {
        "name": "enumcategory_fuelanddriverperformance",
        "value": "F"
      },
      {
        "name": "enumcategory_repairandmaintenance",
        "value": "R"
      }
    ],
    "healthStatus": [
      {
        "name": "enumhealthstatus_noaction",
        "value": "N"
      },
      {
        "name": "enumhealthstatus_servicenow",
        "value": "V"
      },
      {
        "name": "enumhealthstatus_stopnow",
        "value": "T"
      }
    ],
    "otherFilter": [
      {
        "name": "enumhealthstatus_nevermoved",
        "value": "N"
      },
      {
        "name": "enumhealthstatus_driving",
        "value": "D"
      },
      {
        "name": "enumhealthstatus_idle",
        "value": "I"
      },
      {
        "name": "enumhealthstatus_unknown",
        "value": "U"
      },
      {
        "name": "enumhealthstatus_stopped",
        "value": "S"
      }
    ],
    "userPois": [
      {
        "id": 71,
        "organizationId": 36,
        "categoryId": 32,
        "categoryName": "vishal category",
        "subCategoryId": 17,
        "subCategoryName": "ddfd",
        "name": "chaitali222",
        "address": "ulica Sądowa 4, 45-033 Opole, Polska",
        "city": "Opole",
        "country": "POL",
        "zipcode": "45-033",
        "latitude": 50.67305,
        "longitude": 17.92005,
        "state": "Active",
        "createdAt": 1620372307000,
        "icon": "iVBORw0KGgoAAAANSUhEUgAAACAAAAAgCAYAAABzenr0AAACzUlEQVRYR6XXSeiWVRQG8J/QAEktwjAJbBdlk02LWphB5NQiISQ3STTSYK6URIokilqVE6lF1CYJwRbZhNCwyEXzoEU7BbEoXCQFDVA88f3h9fPe977f37N8z3nOed57z3RnmEzmYCVuxuU4bwT/Bd9iH17H0aFuZww0nIWncBfOaGD+wivYgF9b/ocQuBG7cH7L2Zj+J9yBj/pwLQK3YjfOnDD4lPmfuB1v1fB9BK7EJzhrmsGnYH/gBnxd8lMjcBq+GCVaLf5x/DBSXoyze4gmQa/GP+M2NQL3YkfF4SGsxR78PbI5HcvxHC6s4O7DzqEEvsOlBUef4xYcqwQ5F+/jmoL+AC4bQiDH+X3Bwe+Yh8ONnJiLg5hZsLukc23/q0tXcDdeKoA34dGBCfkCVhds78HL3e8lAk/jsQJ4Cd4dSGAx3inYPoP1LQKb8XABPL9WSgXblPBXhe9b8EiLwLOjLB/H34QPB57AQnxQsE2VrGsRuB8vFsAb8cRAAk/i8YLtA9jeIpCj/rIA/hkX4bcGiXPwI2YX7K4av5pSEubbEWT0jsubWNFpQOP6NKQ3cFsBmxF9Af5tnUD0z/eUXKbbg6Na7/pKj9iGTM+SpDTXjCtqrTgdK/27JvmLTzs2WU6uq/SVKR+xSYc9QfqmYWp+0cCka5m9h/SGk6SPwILWMtGK2tHnWj6elEDs30Y64KlIOuLSmoPWRpR7y16Q/WA6kvmfPaCaTy0CCdpXES1SxczvgoYQyKaT7M2YnUQytlNN2ZyqMoRAwMmDvY0y6wZJmS6rTMQTyAwlEFDmQ+bEEEm/T99vyiQEsuF8hmxMfZJF9Vpkg2rKJATi7Ars71nVs4Jfj2+akUcGkxII7E68WgmwCq8NDR676RAIbutoIHVjZRA9NEnwUyGQsZv+ni0pku0nc2PqnTCYx3RPIAHyBshzPJLneu2t0EvmP631ciExHHR4AAAAAElFTkSuQmCC"
      },
      {
        "id": 96,
        "organizationId": 36,
        "categoryId": 32,
        "categoryName": "vishal category",
        "subCategoryId": 17,
        "subCategoryName": "ddfd",
        "name": "aabbcc4",
        "address": "Jeremiášova 2627, 155 00 Praha, Česká Republika",
        "city": "Praha",
        "country": "CZE",
        "zipcode": "155 00",
        "latitude": 50.05687,
        "longitude": 14.31628,
        "state": "Active",
        "createdAt": 1620631053000,
        "icon": "iVBORw0KGgoAAAANSUhEUgAAACAAAAAgCAYAAABzenr0AAACzUlEQVRYR6XXSeiWVRQG8J/QAEktwjAJbBdlk02LWphB5NQiISQ3STTSYK6URIokilqVE6lF1CYJwRbZhNCwyEXzoEU7BbEoXCQFDVA88f3h9fPe977f37N8z3nOed57z3RnmEzmYCVuxuU4bwT/Bd9iH17H0aFuZww0nIWncBfOaGD+wivYgF9b/ocQuBG7cH7L2Zj+J9yBj/pwLQK3YjfOnDD4lPmfuB1v1fB9BK7EJzhrmsGnYH/gBnxd8lMjcBq+GCVaLf5x/DBSXoyze4gmQa/GP+M2NQL3YkfF4SGsxR78PbI5HcvxHC6s4O7DzqEEvsOlBUef4xYcqwQ5F+/jmoL+AC4bQiDH+X3Bwe+Yh8ONnJiLg5hZsLukc23/q0tXcDdeKoA34dGBCfkCVhds78HL3e8lAk/jsQJ4Cd4dSGAx3inYPoP1LQKb8XABPL9WSgXblPBXhe9b8EiLwLOjLB/H34QPB57AQnxQsE2VrGsRuB8vFsAb8cRAAk/i8YLtA9jeIpCj/rIA/hkX4bcGiXPwI2YX7K4av5pSEubbEWT0jsubWNFpQOP6NKQ3cFsBmxF9Af5tnUD0z/eUXKbbg6Na7/pKj9iGTM+SpDTXjCtqrTgdK/27JvmLTzs2WU6uq/SVKR+xSYc9QfqmYWp+0cCka5m9h/SGk6SPwILWMtGK2tHnWj6elEDs30Y64KlIOuLSmoPWRpR7y16Q/WA6kvmfPaCaTy0CCdpXES1SxczvgoYQyKaT7M2YnUQytlNN2ZyqMoRAwMmDvY0y6wZJmS6rTMQTyAwlEFDmQ+bEEEm/T99vyiQEsuF8hmxMfZJF9Vpkg2rKJATi7Ars71nVs4Jfj2+akUcGkxII7E68WgmwCq8NDR676RAIbutoIHVjZRA9NEnwUyGQsZv+ni0pku0nc2PqnTCYx3RPIAHyBshzPJLneu2t0EvmP631ciExHHR4AAAAAElFTkSuQmCC"
      },
      {
        "id": 103,
        "organizationId": 36,
        "categoryId": 32,
        "categoryName": "vishal category",
        "subCategoryId": 0,
        "subCategoryName": "",
        "name": "aa",
        "address": "",
        "city": "Podgorica",
        "country": "MNE",
        "zipcode": "81204",
        "latitude": 42.60935,
        "longitude": 19.50154,
        "state": "Active",
        "createdAt": 1620631433000,
        "icon": "iVBORw0KGgoAAAANSUhEUgAAACAAAAAgCAYAAABzenr0AAACzUlEQVRYR6XXSeiWVRQG8J/QAEktwjAJbBdlk02LWphB5NQiISQ3STTSYK6URIokilqVE6lF1CYJwRbZhNCwyEXzoEU7BbEoXCQFDVA88f3h9fPe977f37N8z3nOed57z3RnmEzmYCVuxuU4bwT/Bd9iH17H0aFuZww0nIWncBfOaGD+wivYgF9b/ocQuBG7cH7L2Zj+J9yBj/pwLQK3YjfOnDD4lPmfuB1v1fB9BK7EJzhrmsGnYH/gBnxd8lMjcBq+GCVaLf5x/DBSXoyze4gmQa/GP+M2NQL3YkfF4SGsxR78PbI5HcvxHC6s4O7DzqEEvsOlBUef4xYcqwQ5F+/jmoL+AC4bQiDH+X3Bwe+Yh8ONnJiLg5hZsLukc23/q0tXcDdeKoA34dGBCfkCVhds78HL3e8lAk/jsQJ4Cd4dSGAx3inYPoP1LQKb8XABPL9WSgXblPBXhe9b8EiLwLOjLB/H34QPB57AQnxQsE2VrGsRuB8vFsAb8cRAAk/i8YLtA9jeIpCj/rIA/hkX4bcGiXPwI2YX7K4av5pSEubbEWT0jsubWNFpQOP6NKQ3cFsBmxF9Af5tnUD0z/eUXKbbg6Na7/pKj9iGTM+SpDTXjCtqrTgdK/27JvmLTzs2WU6uq/SVKR+xSYc9QfqmYWp+0cCka5m9h/SGk6SPwILWMtGK2tHnWj6elEDs30Y64KlIOuLSmoPWRpR7y16Q/WA6kvmfPaCaTy0CCdpXES1SxczvgoYQyKaT7M2YnUQytlNN2ZyqMoRAwMmDvY0y6wZJmS6rTMQTyAwlEFDmQ+bEEEm/T99vyiQEsuF8hmxMfZJF9Vpkg2rKJATi7Ars71nVs4Jfj2+akUcGkxII7E68WgmwCq8NDR676RAIbutoIHVjZRA9NEnwUyGQsZv+ni0pku0nc2PqnTCYx3RPIAHyBshzPJLneu2t0EvmP631ciExHHR4AAAAAElFTkSuQmCC"
      },
      {
        "id": 104,
        "organizationId": 36,
        "categoryId": 32,
        "categoryName": "vishal category",
        "subCategoryId": 0,
        "subCategoryName": "",
        "name": "bb",
        "address": "Waldweg, 3422 Sankt Andrä-Wördern, Österreich",
        "city": "Sankt Andrä-Wördern",
        "country": "",
        "zipcode": "3422",
        "latitude": 48.3333413,
        "longitude": 16.2509485,
        "state": "Active",
        "createdAt": 1620631462000,
        "icon": "iVBORw0KGgoAAAANSUhEUgAAACAAAAAgCAYAAABzenr0AAACzUlEQVRYR6XXSeiWVRQG8J/QAEktwjAJbBdlk02LWphB5NQiISQ3STTSYK6URIokilqVE6lF1CYJwRbZhNCwyEXzoEU7BbEoXCQFDVA88f3h9fPe977f37N8z3nOed57z3RnmEzmYCVuxuU4bwT/Bd9iH17H0aFuZww0nIWncBfOaGD+wivYgF9b/ocQuBG7cH7L2Zj+J9yBj/pwLQK3YjfOnDD4lPmfuB1v1fB9BK7EJzhrmsGnYH/gBnxd8lMjcBq+GCVaLf5x/DBSXoyze4gmQa/GP+M2NQL3YkfF4SGsxR78PbI5HcvxHC6s4O7DzqEEvsOlBUef4xYcqwQ5F+/jmoL+AC4bQiDH+X3Bwe+Yh8ONnJiLg5hZsLukc23/q0tXcDdeKoA34dGBCfkCVhds78HL3e8lAk/jsQJ4Cd4dSGAx3inYPoP1LQKb8XABPL9WSgXblPBXhe9b8EiLwLOjLB/H34QPB57AQnxQsE2VrGsRuB8vFsAb8cRAAk/i8YLtA9jeIpCj/rIA/hkX4bcGiXPwI2YX7K4av5pSEubbEWT0jsubWNFpQOP6NKQ3cFsBmxF9Af5tnUD0z/eUXKbbg6Na7/pKj9iGTM+SpDTXjCtqrTgdK/27JvmLTzs2WU6uq/SVKR+xSYc9QfqmYWp+0cCka5m9h/SGk6SPwILWMtGK2tHnWj6elEDs30Y64KlIOuLSmoPWRpR7y16Q/WA6kvmfPaCaTy0CCdpXES1SxczvgoYQyKaT7M2YnUQytlNN2ZyqMoRAwMmDvY0y6wZJmS6rTMQTyAwlEFDmQ+bEEEm/T99vyiQEsuF8hmxMfZJF9Vpkg2rKJATi7Ars71nVs4Jfj2+akUcGkxII7E68WgmwCq8NDR676RAIbutoIHVjZRA9NEnwUyGQsZv+ni0pku0nc2PqnTCYx3RPIAHyBshzPJLneu2t0EvmP631ciExHHR4AAAAAElFTkSuQmCC"
      },
      {
        "id": 196,
        "organizationId": 36,
        "categoryId": 10,
        "categoryName": "",
        "subCategoryId": 8,
        "subCategoryName": "",
        "name": "PoiTest444",
        "address": "Pune",
        "city": "Pune",
        "country": "India",
        "zipcode": "411057",
        "latitude": 51.07,
        "longitude": 57.07,
        "state": "Active",
        "createdAt": 1621327431000,
        "icon": ""
      },
      {
        "id": 199,
        "organizationId": 36,
        "categoryId": 10,
        "categoryName": "",
        "subCategoryId": 8,
        "subCategoryName": "",
        "name": "Shubh",
        "address": "Pune",
        "city": "Pune",
        "country": "India",
        "zipcode": "411057",
        "latitude": 51.07,
        "longitude": 57.07,
        "state": "Active",
        "createdAt": 1621327937000,
        "icon": ""
      },
      {
        "id": 145,
        "organizationId": 36,
        "categoryId": 32,
        "categoryName": "vishal category",
        "subCategoryId": 17,
        "subCategoryName": "ddfd",
        "name": "Prague",
        "address": "Jeremiášova 2627, 155 00 Praha, Česká Republika",
        "city": "Praha",
        "country": "CZE",
        "zipcode": "155 00",
        "latitude": 50.05687,
        "longitude": 14.31628,
        "state": "Active",
        "createdAt": 1620641373000,
        "icon": "iVBORw0KGgoAAAANSUhEUgAAACAAAAAgCAYAAABzenr0AAACzUlEQVRYR6XXSeiWVRQG8J/QAEktwjAJbBdlk02LWphB5NQiISQ3STTSYK6URIokilqVE6lF1CYJwRbZhNCwyEXzoEU7BbEoXCQFDVA88f3h9fPe977f37N8z3nOed57z3RnmEzmYCVuxuU4bwT/Bd9iH17H0aFuZww0nIWncBfOaGD+wivYgF9b/ocQuBG7cH7L2Zj+J9yBj/pwLQK3YjfOnDD4lPmfuB1v1fB9BK7EJzhrmsGnYH/gBnxd8lMjcBq+GCVaLf5x/DBSXoyze4gmQa/GP+M2NQL3YkfF4SGsxR78PbI5HcvxHC6s4O7DzqEEvsOlBUef4xYcqwQ5F+/jmoL+AC4bQiDH+X3Bwe+Yh8ONnJiLg5hZsLukc23/q0tXcDdeKoA34dGBCfkCVhds78HL3e8lAk/jsQJ4Cd4dSGAx3inYPoP1LQKb8XABPL9WSgXblPBXhe9b8EiLwLOjLB/H34QPB57AQnxQsE2VrGsRuB8vFsAb8cRAAk/i8YLtA9jeIpCj/rIA/hkX4bcGiXPwI2YX7K4av5pSEubbEWT0jsubWNFpQOP6NKQ3cFsBmxF9Af5tnUD0z/eUXKbbg6Na7/pKj9iGTM+SpDTXjCtqrTgdK/27JvmLTzs2WU6uq/SVKR+xSYc9QfqmYWp+0cCka5m9h/SGk6SPwILWMtGK2tHnWj6elEDs30Y64KlIOuLSmoPWRpR7y16Q/WA6kvmfPaCaTy0CCdpXES1SxczvgoYQyKaT7M2YnUQytlNN2ZyqMoRAwMmDvY0y6wZJmS6rTMQTyAwlEFDmQ+bEEEm/T99vyiQEsuF8hmxMfZJF9Vpkg2rKJATi7Ars71nVs4Jfj2+akUcGkxII7E68WgmwCq8NDR676RAIbutoIHVjZRA9NEnwUyGQsZv+ni0pku0nc2PqnTCYx3RPIAHyBshzPJLneu2t0EvmP631ciExHHR4AAAAAElFTkSuQmCC"
      },
      {
        "id": 197,
        "organizationId": 36,
        "categoryId": 10,
        "categoryName": "",
        "subCategoryId": 8,
        "subCategoryName": "",
        "name": "PoiTest",
        "address": "Pune",
        "city": "Pune",
        "country": "India",
        "zipcode": "411057",
        "latitude": 51.07,
        "longitude": 57.07,
        "state": "Active",
        "createdAt": 1621327465000,
        "icon": ""
      },
      {
        "id": 198,
        "organizationId": 36,
        "categoryId": 10,
        "categoryName": "",
        "subCategoryId": 8,
        "subCategoryName": "",
        "name": "PoiTest2",
        "address": "Pune",
        "city": "Pune",
        "country": "India",
        "zipcode": "411057",
        "latitude": 51.07,
        "longitude": 57.07,
        "state": "Active",
        "createdAt": 1621327465000,
        "icon": ""
      },
      {
        "id": 200,
        "organizationId": 36,
        "categoryId": 10,
        "categoryName": "",
        "subCategoryId": 8,
        "subCategoryName": "",
        "name": "PoiTest1111",
        "address": "Pune",
        "city": "Pune",
        "country": "India",
        "zipcode": "411057",
        "latitude": 51.07,
        "longitude": 57.07,
        "state": "Active",
        "createdAt": 1621328633000,
        "icon": ""
      },
      {
        "id": 210,
        "organizationId": 36,
        "categoryId": 10,
        "categoryName": "",
        "subCategoryId": 8,
        "subCategoryName": "",
        "name": "PoiTest123",
        "address": "Pune",
        "city": "Pune",
        "country": "India",
        "zipcode": "411057",
        "latitude": 51.07,
        "longitude": 57.07,
        "state": "Active",
        "createdAt": 1621331429000,
        "icon": ""
      },
      {
        "id": 211,
        "organizationId": 36,
        "categoryId": 10,
        "categoryName": "",
        "subCategoryId": 8,
        "subCategoryName": "",
        "name": "Shubh1",
        "address": "Pune",
        "city": "Pune",
        "country": "India",
        "zipcode": "411057",
        "latitude": 51.07,
        "longitude": 57.07,
        "state": "Active",
        "createdAt": 1621331429000,
        "icon": ""
      },
      {
        "id": 212,
        "organizationId": 36,
        "categoryId": 15,
        "categoryName": "",
        "subCategoryId": 8,
        "subCategoryName": "",
        "name": "shubham2",
        "address": "Indore",
        "city": "Indore",
        "country": "India",
        "zipcode": "411057",
        "latitude": 51.07,
        "longitude": 57.07,
        "state": "Active",
        "createdAt": 1621405700000,
        "icon": ""
      },
      {
        "id": 213,
        "organizationId": 36,
        "categoryId": 15,
        "categoryName": "",
        "subCategoryId": 2,
        "subCategoryName": "Category2",
        "name": "GooteplienPOI",
        "address": "Gooteplie",
        "city": "Brielle",
        "country": "Netherlands",
        "zipcode": "\"3232\"",
        "latitude": 51.9,
        "longitude": 4.16,
        "state": "Active",
        "createdAt": 1621408207000,
        "icon": ""
      },
      {
        "id": 214,
        "organizationId": 36,
        "categoryId": 11,
        "categoryName": "",
        "subCategoryId": 12,
        "subCategoryName": "",
        "name": "HeenwegPOI",
        "address": "Heenweg Westland",
        "city": "Brielle",
        "country": "Netherlands",
        "zipcode": "2691",
        "latitude": 51.98,
        "longitude": 4.18,
        "state": "Active",
        "createdAt": 1621409354000,
        "icon": ""
      },
      {
        "id": 216,
        "organizationId": 36,
        "categoryId": 11,
        "categoryName": "",
        "subCategoryId": 12,
        "subCategoryName": "",
        "name": "HeesssnwegPOI",
        "address": "Heenweg Westland",
        "city": "",
        "country": "",
        "zipcode": "",
        "latitude": 51.98,
        "longitude": 4.18,
        "state": "Active",
        "createdAt": 1621416247000,
        "icon": ""
      },
      {
        "id": 217,
        "organizationId": 36,
        "categoryId": 15,
        "categoryName": "",
        "subCategoryId": 2,
        "subCategoryName": "Category2",
        "name": "GooqqqqteplienPOI",
        "address": "Gooteplie",
        "city": "",
        "country": "",
        "zipcode": "",
        "latitude": 51.9,
        "longitude": 4.16,
        "state": "Active",
        "createdAt": 1621416247000,
        "icon": ""
      },
      {
        "id": 230,
        "organizationId": 36,
        "categoryId": 90,
        "categoryName": "BulkImportGeofence",
        "subCategoryId": 17,
        "subCategoryName": "ddfd",
        "name": "testTest",
        "address": "Bieno, Trentino-Alto Adige, Italia",
        "city": "Bieno",
        "country": "ITA",
        "zipcode": "38050",
        "latitude": 46.1223,
        "longitude": 11.5658,
        "state": "Active",
        "createdAt": 1622544222000,
        "icon": ""
      },
      {
        "id": 231,
        "organizationId": 36,
        "categoryId": 90,
        "categoryName": "BulkImportGeofence",
        "subCategoryId": 17,
        "subCategoryName": "ddfd",
        "name": "sss",
        "address": "08261 Schöneck/Vogtl., Deutschland",
        "city": "Schöneck/Vogtl.",
        "country": "DEU",
        "zipcode": "08261",
        "latitude": 50.40772,
        "longitude": 12.34504,
        "state": "Active",
        "createdAt": 1622546091000,
        "icon": ""
      },
      {
        "id": 229,
        "organizationId": 36,
        "categoryId": 32,
        "categoryName": "vishal category",
        "subCategoryId": 75,
        "subCategoryName": "vish-subby",
        "name": "shsh",
        "address": "Ménaka, Mali",
        "city": "Ménaka",
        "country": "MLI",
        "zipcode": "",
        "latitude": 17.3616,
        "longitude": 3.904,
        "state": "Active",
        "createdAt": 1622444997000,
        "icon": "iVBORw0KGgoAAAANSUhEUgAAACAAAAAgCAYAAABzenr0AAACzUlEQVRYR6XXSeiWVRQG8J/QAEktwjAJbBdlk02LWphB5NQiISQ3STTSYK6URIokilqVE6lF1CYJwRbZhNCwyEXzoEU7BbEoXCQFDVA88f3h9fPe977f37N8z3nOed57z3RnmEzmYCVuxuU4bwT/Bd9iH17H0aFuZww0nIWncBfOaGD+wivYgF9b/ocQuBG7cH7L2Zj+J9yBj/pwLQK3YjfOnDD4lPmfuB1v1fB9BK7EJzhrmsGnYH/gBnxd8lMjcBq+GCVaLf5x/DBSXoyze4gmQa/GP+M2NQL3YkfF4SGsxR78PbI5HcvxHC6s4O7DzqEEvsOlBUef4xYcqwQ5F+/jmoL+AC4bQiDH+X3Bwe+Yh8ONnJiLg5hZsLukc23/q0tXcDdeKoA34dGBCfkCVhds78HL3e8lAk/jsQJ4Cd4dSGAx3inYPoP1LQKb8XABPL9WSgXblPBXhe9b8EiLwLOjLB/H34QPB57AQnxQsE2VrGsRuB8vFsAb8cRAAk/i8YLtA9jeIpCj/rIA/hkX4bcGiXPwI2YX7K4av5pSEubbEWT0jsubWNFpQOP6NKQ3cFsBmxF9Af5tnUD0z/eUXKbbg6Na7/pKj9iGTM+SpDTXjCtqrTgdK/27JvmLTzs2WU6uq/SVKR+xSYc9QfqmYWp+0cCka5m9h/SGk6SPwILWMtGK2tHnWj6elEDs30Y64KlIOuLSmoPWRpR7y16Q/WA6kvmfPaCaTy0CCdpXES1SxczvgoYQyKaT7M2YnUQytlNN2ZyqMoRAwMmDvY0y6wZJmS6rTMQTyAwlEFDmQ+bEEEm/T99vyiQEsuF8hmxMfZJF9Vpkg2rKJATi7Ars71nVs4Jfj2+akUcGkxII7E68WgmwCq8NDR676RAIbutoIHVjZRA9NEnwUyGQsZv+ni0pku0nc2PqnTCYx3RPIAHyBshzPJLneu2t0EvmP631ciExHHR4AAAAAElFTkSuQmCC"
      }
    ],
    "globalPois": [
      {
        "id": 236,
        "organizationId": 0,
        "categoryId": 104,
        "categoryName": "Global Test",
        "subCategoryId": 105,
        "subCategoryName": "Global Test - Sub",
        "name": "Test POI",
        "address": "28 Rue Pierre Loti, 92340 Bourg-la-Reine, France",
        "city": "Bourg-la-Reine",
        "country": "FRA",
        "zipcode": "92340",
        "latitude": 48.78332,
        "longitude": 2.31167,
        "state": "Active",
        "createdAt": 1623224566000,
        "icon": "iVBORw0KGgoAAAANSUhEUgAAACAAAAAgCAYAAABzenr0AAACzUlEQVRYR6XXSeiWVRQG8J/QAEktwjAJbBdlk02LWphB5NQiISQ3STTSYK6URIokilqVE6lF1CYJwRbZhNCwyEXzoEU7BbEoXCQFDVA88f3h9fPe977f37N8z3nOed57z3RnmEzmYCVuxuU4bwT/Bd9iH17H0aFuZww0nIWncBfOaGD+wivYgF9b/ocQuBG7cH7L2Zj+J9yBj/pwLQK3YjfOnDD4lPmfuB1v1fB9BK7EJzhrmsGnYH/gBnxd8lMjcBq+GCVaLf5x/DBSXoyze4gmQa/GP+M2NQL3YkfF4SGsxR78PbI5HcvxHC6s4O7DzqEEvsOlBUef4xYcqwQ5F+/jmoL+AC4bQiDH+X3Bwe+Yh8ONnJiLg5hZsLukc23/q0tXcDdeKoA34dGBCfkCVhds78HL3e8lAk/jsQJ4Cd4dSGAx3inYPoP1LQKb8XABPL9WSgXblPBXhe9b8EiLwLOjLB/H34QPB57AQnxQsE2VrGsRuB8vFsAb8cRAAk/i8YLtA9jeIpCj/rIA/hkX4bcGiXPwI2YX7K4av5pSEubbEWT0jsubWNFpQOP6NKQ3cFsBmxF9Af5tnUD0z/eUXKbbg6Na7/pKj9iGTM+SpDTXjCtqrTgdK/27JvmLTzs2WU6uq/SVKR+xSYc9QfqmYWp+0cCka5m9h/SGk6SPwILWMtGK2tHnWj6elEDs30Y64KlIOuLSmoPWRpR7y16Q/WA6kvmfPaCaTy0CCdpXES1SxczvgoYQyKaT7M2YnUQytlNN2ZyqMoRAwMmDvY0y6wZJmS6rTMQTyAwlEFDmQ+bEEEm/T99vyiQEsuF8hmxMfZJF9Vpkg2rKJATi7Ars71nVs4Jfj2+akUcGkxII7E68WgmwCq8NDR676RAIbutoIHVjZRA9NEnwUyGQsZv+ni0pku0nc2PqnTCYx3RPIAHyBshzPJLneu2t0EvmP631ciExHHR4AAAAAElFTkSuQmCC"
      },
      {
        "id": 237,
        "organizationId": 0,
        "categoryId": 106,
        "categoryName": "circular POI04",
        "subCategoryId": 105,
        "subCategoryName": "Global Test - Sub",
        "name": "Test POI2",
        "address": "88 Colworth Road, London, E11 1JD, United Kingdom",
        "city": "London",
        "country": "GBR",
        "zipcode": "E11 1JD",
        "latitude": 51.57404,
        "longitude": 0.00833,
        "state": "Active",
        "createdAt": 1623224632000,
        "icon": "iVBORw0KGgoAAAANSUhEUgAAACAAAAAgCAYAAABzenr0AAACzUlEQVRYR6XXSeiWVRQG8J/QAEktwjAJbBdlk02LWphB5NQiISQ3STTSYK6URIokilqVE6lF1CYJwRbZhNCwyEXzoEU7BbEoXCQFDVA88f3h9fPe977f37N8z3nOed57z3RnmEzmYCVuxuU4bwT/Bd9iH17H0aFuZww0nIWncBfOaGD+wivYgF9b/ocQuBG7cH7L2Zj+J9yBj/pwLQK3YjfOnDD4lPmfuB1v1fB9BK7EJzhrmsGnYH/gBnxd8lMjcBq+GCVaLf5x/DBSXoyze4gmQa/GP+M2NQL3YkfF4SGsxR78PbI5HcvxHC6s4O7DzqEEvsOlBUef4xYcqwQ5F+/jmoL+AC4bQiDH+X3Bwe+Yh8ONnJiLg5hZsLukc23/q0tXcDdeKoA34dGBCfkCVhds78HL3e8lAk/jsQJ4Cd4dSGAx3inYPoP1LQKb8XABPL9WSgXblPBXhe9b8EiLwLOjLB/H34QPB57AQnxQsE2VrGsRuB8vFsAb8cRAAk/i8YLtA9jeIpCj/rIA/hkX4bcGiXPwI2YX7K4av5pSEubbEWT0jsubWNFpQOP6NKQ3cFsBmxF9Af5tnUD0z/eUXKbbg6Na7/pKj9iGTM+SpDTXjCtqrTgdK/27JvmLTzs2WU6uq/SVKR+xSYc9QfqmYWp+0cCka5m9h/SGk6SPwILWMtGK2tHnWj6elEDs30Y64KlIOuLSmoPWRpR7y16Q/WA6kvmfPaCaTy0CCdpXES1SxczvgoYQyKaT7M2YnUQytlNN2ZyqMoRAwMmDvY0y6wZJmS6rTMQTyAwlEFDmQ+bEEEm/T99vyiQEsuF8hmxMfZJF9Vpkg2rKJATi7Ars71nVs4Jfj2+akUcGkxII7E68WgmwCq8NDR676RAIbutoIHVjZRA9NEnwUyGQsZv+ni0pku0nc2PqnTCYx3RPIAHyBshzPJLneu2t0EvmP631ciExHHR4AAAAAElFTkSuQmCC"
      }
    ],
    "driverList": [
      {
        "driverId": "B  B116366983456001",
        "firstName": "Driver1",
        "lastName": "DriverL1",
        "organizationId": 36
      },
      {
        "driverId": "NL B000171984000002",
        "firstName": "Driver2",
        "lastName": "DriverL1",
        "organizationId": 36
      },
      {
        "driverId": "SK 1116526558846037",
        "firstName": "Neeraj",
        "lastName": "Lohumi",
        "organizationId": 36
      },
      {
        "driverId": "B000384974000000",
        "firstName": "jacob",
        "lastName": "Wersint",
        "organizationId": 12
      },
      {
        "driverId": "NL B000384974000000",
        "firstName": "Hero",
        "lastName": "Honda",
        "organizationId": 36
      },
      {
        "driverId": "SK 2236526558846039",
        "firstName": "Sid",
        "lastName": "U",
        "organizationId": 36
      }
    ]
  };

  constructor() { }

  ngOnInit(): void {
  }

  tabVisibilityHandler(tabVisibility: boolean){
    this.tabVisibilityStatus = tabVisibility;
  }
  
  onTabChanged(event: any){
    this.selectedIndex = event.index;
  }
  
}
