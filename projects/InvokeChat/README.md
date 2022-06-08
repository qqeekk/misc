# Demo Project To Test AWS GameLift

This is the simple chat application just to test AWS GamLift functionality.

## Start

Download `GameLiftLocal.jar`. It can be found by following URLs:

- https://aws.amazon.com/ru/gamelift/getting-started/
- https://gamelift-release.s3-us-west-2.amazonaws.com/GameLift_06_03_2021.zip

The start client and server:

```bash
$ sudo archlinux-java set java-8-jre/jre
$ java -jar GameLiftLocal.jar -p 9080
$ ./InvokeChat.Backend.Host.Aws/bin/Debug/net6.0/chathostaws
$ ./InvokeChat.Client.Aws/bin/Debug/net6.0/chatclientaws
```

## Publish

- `dotnet publish ./InvokeChat.Backend.Host.Aws/InvokeChat.Backend.Host.Aws.csproj -r linux-x64 -c Release --self-contained -p:PublishSingleFile=true -o ./build`
- `dotnet publish ./InvokeChat.Client.Aws/InvokeChat.Client.Aws.csproj -r linux-x64 -c Release --self-contained -p:PublishSingleFile=true -o ./build`
- `dotnet publish ./WebSocketTest/WebSocketTest.csproj -r linux-x64 -c Release --self-contained -p:PublishSingleFile=true -p:PublishTrimmed=true -o ./build`

## AWS GameLift Commands

**Upload build**

- `aws gamelift upload-build --name invokechat --build-version "0.1.0" --build-root ./build --operating-system AMAZON_LINUX_2 --region us-west-2`
- `aws gamelift update-build --build-id build-X`

**Get SSH access**

```bash
set FLEETID "fleet-4bb6b510-8ebd-441a-b50c-87cd6d8ebead"
set INSTANCEID $(aws gamelift describe-instances --fleet-id $FLEETID --query "Instances[0].InstanceId" --output text)
set INSTANCEDNS $(aws gamelift describe-instances --fleet-id $FLEETID --query "Instances[0].DnsName" --output text)
set IPADDRESS $(curl ifconfig.co/)
rm privkey.pem -f
aws gamelift get-instance-access --fleet-id $FLEETID --instance-id $INSTANCEID --query "InstanceAccess.Credentials.Secret" --output text > privkey.pem
chmod 400 privkey.pem
aws gamelift update-fleet-port-settings --fleet-id $FLEETID --inbound-permission-authorizations "FromPort=22,ToPort=22,IpRange=$IPADDRESS/32,Protocol=TCP"
ssh -i privkey.pem gl-user-remote@$INSTANCEDNS
```

## Links

- https://docs.aws.amazon.com/gamelift/latest/developerguide/gamelift-sdk-server-api-interaction-vsd.html
- https://docs.aws.amazon.com/gamelift/latest/developerguide/integration-testing-local.html
