#import <Foundation/Foundation.h>
#import <CoreLocation/CoreLocation.h>
#import <EstimoteSDK/EstimoteSDK.h>

extern void UnitySendMessage(const char *, const char *, const char *);

@interface EstimoteUnity : NSObject <ESTBeaconManagerDelegate>

@property (nonatomic) ESTBeaconManager *beaconManager;
@property (nonatomic) NSMutableArray *cachedBeaconRegions;
@property (nonatomic) BOOL hasInitializedEstimote;
@property (nonatomic) BOOL hasInitializedEstimoteCloud;
@property (nonatomic) NSString* unityGameObjectName;

@end

