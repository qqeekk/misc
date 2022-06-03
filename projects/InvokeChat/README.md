# Demo Project To Test AWS GameLift

This is the simple chat application just to test AWS GamLift functionality.

## Start

Download `GameLiftLocal.jar`. It can be found by following URLs:

- https://aws.amazon.com/ru/gamelift/getting-started/
- https://gamelift-release.s3-us-west-2.amazonaws.com/GameLift_06_03_2021.zip

The start client and server:

```
0> sudo archlinux-java set java-8-jre/jre
1> java -jar GameLiftLocal.jar -p 9080
2> ./InvokeChat.Backend.Host.Aws/bin/Debug/net6.0/chathostaws
3> ./InvokeChat.Client.Aws/bin/Debug/net6.0/chatclientaws
```

## Links

- https://docs.aws.amazon.com/gamelift/latest/developerguide/gamelift-sdk-server-api-interaction-vsd.html
- https://docs.aws.amazon.com/gamelift/latest/developerguide/integration-testing-local.html
