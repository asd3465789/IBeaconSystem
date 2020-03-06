#import "EstimoteUnity.h"

@implementation EstimoteUnity

- (void) initEstimote: (NSString *)gameObjectName beaconRegions:(NSMutableArray *)beaconRegions
{
    self.beaconManager = [ESTBeaconManager new];
    self.beaconManager.delegate = self;
    
    self.unityGameObjectName = gameObjectName;
    
    self.cachedBeaconRegions = [NSMutableArray new];
    for(int i = 0; i < [beaconRegions count]; i++) {
        NSString *beaconUUID = [beaconRegions objectAtIndex:i];
        [self.cachedBeaconRegions addObject:[[CLBeaconRegion alloc] initWithProximityUUID:[[NSUUID alloc] initWithUUIDString: beaconUUID] identifier: beaconUUID]];
    }
    
    [self.beaconManager requestAlwaysAuthorization];
    self.hasInitializedEstimote = YES;
    
    UnitySendMessage([self.unityGameObjectName UTF8String], "InitEstimoteCompleteCallback", "");
}

- (void) initEstimoteCloud: (NSString *) appId appToken:(NSString *)appToken
{
    [ESTConfig setupAppID:appId andAppToken:appToken];
    self.hasInitializedEstimoteCloud = YES;
    
    UnitySendMessage([self.unityGameObjectName UTF8String], "InitEstimoteCloudCompleteCallback", "");
}

- (void) startScanning
{
    for(int i = 0; i < [self.cachedBeaconRegions count]; i++) {
        [self.beaconManager startRangingBeaconsInRegion: [self.cachedBeaconRegions objectAtIndex:i]];
    }
    
    UnitySendMessage([self.unityGameObjectName UTF8String], "StartedScanningCallback", "");
}

- (void) stopScanning
{
    [self.beaconManager stopRangingBeaconsInAllRegions];
    
    UnitySendMessage([self.unityGameObjectName UTF8String], "StoppedScanningCallback", "");
}

- (void)beaconManager:(id)manager didRangeBeacons:(NSArray<CLBeacon *> *)beacons inRegion:(CLBeaconRegion *)region
{
    NSMutableArray* beaconsArray = [[NSMutableArray alloc] init];
    
    for(CLBeacon *beacon in beacons) {
        // Create the beacon dictionary which will convert into JSON
        NSMutableDictionary* beaconDictionary = [[NSMutableDictionary alloc] init];
        [beaconDictionary setObject:beacon.proximityUUID.UUIDString forKey:@"UUID"];
        [beaconDictionary setObject:beacon.major forKey:@"Major"];
        [beaconDictionary setObject:beacon.minor forKey:@"Minor"];
        [beaconDictionary setObject:[NSNumber numberWithInt:beacon.proximity] forKey:@"BeaconRange"];
        [beaconDictionary setObject:[NSNumber numberWithInteger:beacon.rssi] forKey:@"RSSI"];
        [beaconDictionary setObject:[NSNumber numberWithDouble:beacon.accuracy] forKey:@"Accuracy"];
        
        // Add the beacon dictionary into the beacons array
        [beaconsArray addObject: beaconDictionary];
    }
    
    NSData* beaconsData = [NSJSONSerialization dataWithJSONObject:beaconsArray options:NSJSONWritingPrettyPrinted error:nil];
    NSString* json = [[NSString alloc] initWithData:beaconsData encoding:NSUTF8StringEncoding];
    UnitySendMessage([self.unityGameObjectName UTF8String],"DidRangeBeaconsCallback", [[NSString stringWithString:json] cStringUsingEncoding:NSUTF8StringEncoding]);
}

- (void) getBeaconCloudDetails: (NSString *) beaconUUID beaconMajor:(int)beaconMajor beaconMinor:(int)beaconMinor
{
    NSString* beaconIdentifier =[NSString stringWithFormat:@"%@:%d:%d", beaconUUID, beaconMajor, beaconMinor];
    NSMutableArray* beaconsArray = [[NSMutableArray alloc] init];
    [beaconsArray addObject: beaconIdentifier];
    
    ESTRequestGetBeaconsDetails *request = [[ESTRequestGetBeaconsDetails alloc] initWithMacAddresses:beaconsArray andFields:(ESTBeaconDetailsAllFields)];
    [request sendRequestWithCompletion:^(NSArray<ESTBeaconVO *> *beaconVOs, NSError *error) {
        if(error) {
            UnitySendMessage([self.unityGameObjectName UTF8String], "FetchBeaconDetailsFailureCallback", [[error localizedDescription] UTF8String]);
            return;
        }
        
        ESTBeaconVO *beaconVO = [beaconVOs objectAtIndex:0];
        NSMutableDictionary* beaconDictionary = [[NSMutableDictionary alloc] init];
        [beaconDictionary setObject:beaconVO.proximityUUID forKey:@"UUID"];
        [beaconDictionary setObject:beaconVO.major forKey:@"Major"];
        [beaconDictionary setObject:beaconVO.minor forKey:@"Minor"];
        [beaconDictionary setObject:beaconVO.name forKey:@"BeaconName"];
        [beaconDictionary setObject:ConvertEstimoteColorToString(beaconVO.color) forKey:@"BeaconColor"];
        [beaconDictionary setObject:beaconVO.batteryLifeExpectancy forKey:@"BatteryLifeInDays"];
        [beaconDictionary setObject:beaconVO.macAddress forKey:@"MacAddress"];
        
        NSData* beaconsData = [NSJSONSerialization dataWithJSONObject:beaconDictionary options:NSJSONWritingPrettyPrinted error:nil];
        NSString* json = [[NSString alloc] initWithData:beaconsData encoding:NSUTF8StringEncoding];
        UnitySendMessage([self.unityGameObjectName UTF8String],"FetchBeaconDetailsSuccessCallback", [[NSString stringWithString:json] cStringUsingEncoding:NSUTF8StringEncoding]);
    }];
}

NSString* ConvertEstimoteColorToString (ESTColor color)
{
    NSString* colorString;
    
    if(color == ESTColorUnknown) {
        colorString = @"Unknown";
    }
    if(color == ESTColorMintCocktail) {
        colorString = @"Mint Cocktail";
    }
    if(color == ESTColorIcyMarshmallow) {
        colorString = @"Icy Marshmallow";
    }
    if(color == ESTColorBlueberryPie) {
        colorString = @"Blueberry Pie";
    }
    if(color == ESTColorSweetBeetroot) {
        colorString = @"Sweet Beetroot";
    }
    if(color == ESTColorCandyFloss) {
        colorString = @"Candy Floss";
    }
    if(color == ESTColorLemonTart) {
        colorString = @"Lemon Tart";
    }
    if(color == ESTColorVanillaJello) {
        colorString = @"Vanilla Jello";
    }
    if(color == ESTColorLiquoriceSwirl) {
        colorString = @"Liquorice Swirl";
    }
    if(color == ESTColorWhite) {
        colorString = @"White";
    }
    if(color == ESTColorBlack) {
        colorString = @"Black";
    }
    if(color == ESTColorCoconutPuff) {
        colorString = @"Coconut Puff";
    }
    if(color == ESTColorTransparent) {
        colorString = @"Transparent";
    }
    
    return colorString;
}

@end

extern "C" {
    EstimoteUnity *sInstance;
    
    void IOS_Initialize(char* gameObjectName, char* beaconRegions[], int beaconRegionsLength)
    {
        NSMutableArray *myBeaconRegions = [NSMutableArray new];
        for(int i = 0; i < beaconRegionsLength; i++) {
            NSString* beaconRegionString = [NSString stringWithUTF8String:beaconRegions[i]];
            [myBeaconRegions addObject:beaconRegionString];
        }
        
        if(sInstance == nil) {
            // Create our instance
            sInstance = [EstimoteUnity alloc];
            
            if(beaconRegionsLength < 0) {
                NSLog(@"Cannot initialise without any beacon regions.");
                return;
            }
        }
        [sInstance initEstimote:[NSString stringWithUTF8String:gameObjectName] beaconRegions:myBeaconRegions];
    }
    
    void IOS_InitializeEstimoteCloud(char* appId, char* appToken)
    {
        [sInstance initEstimoteCloud:[NSString stringWithUTF8String:appId] appToken:[NSString stringWithUTF8String:appToken]];
    }
    
    void IOS_StartEstimoteScanning()
    {
        if(sInstance != nil) {
            if(sInstance.hasInitializedEstimote == NO) {
                return;
            }
            [sInstance startScanning];
        }
    }
    
    void IOS_StopEstimoteScanning()
    {
        if(sInstance != nil) {
            [sInstance stopScanning];
        }
    }
    
    int IOS_CheckDeviceSupportsBeacons()
    {
        // Check to see if the device supports beacons
        if(![CLLocationManager isRangingAvailable]) {
            return 0;
        }
        
        return 1;
    }
    
    void IOS_GetBeaconCloudDetails(char* beaconUUID, int beaconMajor, int beaconMinor)
    {
        if(!sInstance.hasInitializedEstimoteCloud) {
            return;
        }
        [sInstance getBeaconCloudDetails:[NSString stringWithUTF8String:beaconUUID] beaconMajor:beaconMajor beaconMinor:beaconMinor];
    }
}

