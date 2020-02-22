using System.Linq;
using NKMCore.Hex;
using Xunit;

namespace NKMCore.Tests
{
    public class HexMapSerializerTests
    {
        [Fact]
        public void SingleTileDeserialize()
        {
            const string mapString = "TestName\n\n0:0;Normal\n\nSpawnPoint1";

            HexMap map = HexMapSerializer.Deserialize(mapString);
            Assert.Single(map.Cells);
            Assert.Equal("TestName", map.Name);
            Assert.Equal(HexCell.TileType.Normal, map.Cells.First().Type);
            Assert.Equal(new HexCoordinates(0, 0), map.Cells.First().Coordinates);
        }

        [Theory]
        [InlineData("TestName\n\n0:0;Normal\n\nSpawnPoint1")]
        [InlineData("TestName\n\n1:1;Wall\n\nSpawnPoint1")]
        [InlineData("TestName\n\n0:0;Normal\n\nSpawnPoint1;SpawnPoint2")]
        [InlineData(
@"TestName

0:0;Wall
1:0;Wall
2:0;Wall
3:0;Wall
4:0;Wall
5:0;Wall
6:0;Wall
7:0;Wall
8:0;Wall
9:0;Wall
10:0;Wall
11:0;Wall
12:0;Wall
13:0;Wall
14:0;Wall
15:0;Wall
16:0;Wall
17:0;Wall
18:0;Wall
19:0;Wall
20:0;Wall
21:0;Wall
22:0;Wall
23:0;Wall
24:0;Wall
25:0;Wall
26:0;Wall
27:0;Wall
28:0;Wall
29:0;Wall
30:0;Wall
31:0;Wall
32:0;Wall
33:0;Wall
34:0;Wall
35:0;Wall
0:1;Wall
1:1;Wall
2:1;Wall
3:1;Wall
4:1;Wall
5:1;Wall
6:1;Wall
7:1;Wall
8:1;Wall
9:1;Wall
10:1;Wall
11:1;Normal
12:1;Normal
13:1;Normal
14:1;Wall
15:1;Normal
16:1;Normal
17:1;Normal
18:1;Normal
19:1;Normal
20:1;Normal
21:1;Normal
22:1;Normal
23:1;Normal
24:1;Wall
25:1;Wall
26:1;Wall
27:1;Wall
28:1;Wall
29:1;Wall
30:1;Wall
31:1;Wall
32:1;Wall
33:1;Wall
34:1;Wall
35:1;Wall
-1:2;Wall
0:2;Wall
1:2;Wall
2:2;Wall
3:2;Wall
4:2;Wall
5:2;Wall
6:2;Wall
7:2;Wall
8:2;Wall
9:2;Wall
10:2;Normal
11:2;Normal
12:2;Normal
13:2;Wall
14:2;Normal
15:2;Normal
16:2;Normal
17:2;Normal
18:2;Normal
19:2;Normal
20:2;Normal
21:2;Normal
22:2;Normal
23:2;Normal
24:2;Wall
25:2;Wall
26:2;Wall
27:2;Wall
28:2;Wall
29:2;Wall
30:2;Wall
31:2;Wall
32:2;Wall
33:2;Wall
34:2;Wall
-1:3;Wall
0:3;Wall
1:3;Wall
2:3;Wall
3:3;Wall
4:3;Wall
5:3;Wall
6:3;Wall
7:3;Wall
8:3;Wall
9:3;Normal
10:3;Normal
11:3;Normal
12:3;Normal
13:3;Wall
14:3;Normal
15:3;Normal
16:3;Normal
17:3;Normal
18:3;Normal
19:3;Normal
20:3;Normal
21:3;Normal
22:3;Normal
23:3;Normal
24:3;Wall
25:3;Wall
26:3;Wall
27:3;Wall
28:3;Wall
29:3;Wall
30:3;Wall
31:3;Wall
32:3;Wall
33:3;Wall
34:3;Wall
-2:4;Wall
-1:4;Wall
0:4;Wall
1:4;Wall
2:4;Wall
3:4;Wall
4:4;Wall
5:4;Wall
6:4;Wall
7:4;Wall
8:4;Normal
9:4;Normal
10:4;Normal
11:4;Normal
12:4;Normal
13:4;Normal
14:4;Normal
15:4;Normal
16:4;Normal
17:4;Normal
18:4;Normal
19:4;Normal
20:4;Normal
21:4;Normal
22:4;Normal
23:4;Normal
24:4;Wall
25:4;Wall
26:4;Wall
27:4;Wall
28:4;Wall
29:4;Wall
30:4;Wall
31:4;Wall
32:4;Wall
33:4;Wall
-2:5;Wall
-1:5;Wall
0:5;Wall
1:5;Wall
2:5;Wall
3:5;Wall
4:5;Wall
5:5;Wall
6:5;Wall
7:5;Normal
8:5;Normal
9:5;Normal
10:5;Normal
11:5;Normal
12:5;Normal
13:5;Normal
14:5;Normal
15:5;Normal
16:5;Normal
17:5;Normal
18:5;Normal
19:5;Normal
20:5;Normal
21:5;Normal
22:5;Normal
23:5;Normal
24:5;Wall
25:5;Wall
26:5;Wall
27:5;Wall
28:5;Wall
29:5;Wall
30:5;Wall
31:5;Wall
32:5;Wall
33:5;Wall
-3:6;Wall
-2:6;Wall
-1:6;Wall
0:6;Wall
1:6;Wall
2:6;Wall
3:6;Wall
4:6;Wall
5:6;Wall
6:6;Normal
7:6;Normal
8:6;Normal
9:6;Normal
10:6;Normal
11:6;Normal
12:6;Normal
13:6;Normal
14:6;Normal
15:6;Normal
16:6;Normal
17:6;Normal
18:6;Normal
19:6;Normal
20:6;Normal
21:6;Normal
22:6;Normal
23:6;Normal
24:6;Wall
25:6;Wall
26:6;Wall
27:6;Wall
28:6;Wall
29:6;Wall
30:6;Wall
31:6;Wall
32:6;Wall
-3:7;Wall
-2:7;Wall
-1:7;Wall
0:7;Wall
1:7;Wall
2:7;Wall
3:7;Wall
4:7;Wall
5:7;Normal
6:7;Normal
7:7;Normal
8:7;Normal
9:7;Wall
10:7;Normal
11:7;Wall
12:7;Wall
13:7;Wall
14:7;Wall
15:7;Normal
16:7;Wall
17:7;Wall
18:7;Wall
19:7;Normal
20:7;Normal
21:7;Normal
22:7;Normal
23:7;Normal
24:7;Wall
25:7;Wall
26:7;Wall
27:7;Wall
28:7;Wall
29:7;Wall
30:7;Wall
31:7;Wall
32:7;Wall
-4:8;Wall
-3:8;Wall
-2:8;Wall
-1:8;Wall
0:8;Wall
1:8;Wall
2:8;Wall
3:8;Wall
4:8;Normal
5:8;Normal
6:8;Normal
7:8;Normal
8:8;Wall
9:8;Normal
10:8;Normal
11:8;Normal
12:8;Normal
13:8;Normal
14:8;Normal
15:8;Normal
16:8;Normal
17:8;Normal
18:8;Normal
19:8;Normal
20:8;Normal
21:8;Normal
22:8;Normal
23:8;Normal
24:8;Wall
25:8;Wall
26:8;Wall
27:8;Wall
28:8;Wall
29:8;Wall
30:8;Wall
31:8;Wall
-4:9;Wall
-3:9;Wall
-2:9;Wall
-1:9;Wall
0:9;Wall
1:9;Wall
2:9;Wall
3:9;Normal
4:9;Normal
5:9;Normal
6:9;Normal
7:9;Wall
8:9;Normal
9:9;Normal
10:9;Normal
11:9;Normal
12:9;Normal
13:9;Normal
14:9;Normal
15:9;Normal
16:9;Normal
17:9;Normal
18:9;Normal
19:9;Wall
20:9;Normal
21:9;Normal
22:9;Normal
23:9;Normal
24:9;Wall
25:9;Wall
26:9;Wall
27:9;Wall
28:9;Wall
29:9;Wall
30:9;Wall
31:9;Wall
-5:10;Wall
-4:10;Wall
-3:10;Wall
-2:10;Wall
-1:10;Wall
0:10;Wall
1:10;Wall
2:10;Normal
3:10;Normal
4:10;Normal
5:10;Normal
6:10;Wall
7:10;Normal
8:10;Normal
9:10;Normal
10:10;Normal
11:10;Normal
12:10;Normal
13:10;Normal
14:10;Normal
15:10;Normal
16:10;Normal
17:10;Normal
18:10;Normal
19:10;Wall
20:10;Normal
21:10;Normal
22:10;Normal
23:10;Normal
24:10;Wall
25:10;Wall
26:10;Wall
27:10;Wall
28:10;Wall
29:10;Wall
30:10;Wall
-5:11;Wall
-4:11;Wall
-3:11;Wall
-2:11;Wall
-1:11;Wall
0:11;Wall
1:11;Normal
2:11;Normal
3:11;Normal
4:11;Normal
5:11;Wall
6:11;Normal
7:11;Normal
8:11;Normal
9:11;Normal
10:11;Normal
11:11;Normal
12:11;Normal
13:11;Wall
14:11;Normal
15:11;Normal
16:11;Normal
17:11;Normal
18:11;Normal
19:11;Normal
20:11;Normal
21:11;Normal
22:11;Normal
23:11;Normal
24:11;Wall
25:11;Wall
26:11;Wall
27:11;Wall
28:11;Wall
29:11;Wall
30:11;Wall
-6:12;Wall
-5:12;Wall
-4:12;Wall
-3:12;Wall
-2:12;Wall
-1:12;Wall
0:12;Normal
1:12;Normal
2:12;Normal
3:12;Normal
4:12;Wall
5:12;Normal
6:12;Normal
7:12;Normal
8:12;Normal
9:12;Normal
10:12;Normal
11:12;Normal
12:12;Normal
13:12;Normal
14:12;Normal
15:12;Normal
16:12;Normal
17:12;Normal
18:12;Normal
19:12;Normal
20:12;Normal
21:12;Normal
22:12;Normal
23:12;Normal
24:12;Wall
25:12;Wall
26:12;Wall
27:12;Wall
28:12;Wall
29:12;Wall
-6:13;Wall
-5:13;Wall
-4:13;Wall
-3:13;Wall
-2:13;Wall
-1:13;Normal
0:13;Normal
1:13;Normal
2:13;Normal
3:13;Wall
4:13;Normal
5:13;Normal
6:13;Normal
7:13;Normal
8:13;Normal
9:13;Normal
10:13;Normal
11:13;Normal
12:13;Wall
13:13;Normal
14:13;Normal
15:13;Normal
16:13;Normal
17:13;Normal
18:13;Normal
19:13;Wall
20:13;Normal
21:13;Normal
22:13;Normal
23:13;Normal
24:13;Wall
25:13;Wall
26:13;Wall
27:13;Wall
28:13;Wall
29:13;Wall
-7:14;Wall
-6:14;Wall
-5:14;Wall
-4:14;Wall
-3:14;Wall
-2:14;Normal
-1:14;Normal
0:14;Normal
1:14;Normal
2:14;Wall
3:14;Normal
4:14;Normal
5:14;Normal
6:14;Normal
7:14;Normal
8:14;Normal
9:14;Normal
10:14;Normal
11:14;Wall
12:14;Normal
13:14;Normal
14:14;Normal
15:14;Normal
16:14;Normal
17:14;Normal
18:14;Normal
19:14;Wall
20:14;Normal
21:14;Normal
22:14;Normal
23:14;Normal
24:14;Wall
25:14;Wall
26:14;Wall
27:14;Wall
28:14;Wall
-7:15;Wall
-6:15;Wall
-5:15;Wall
-4:15;Wall
-3:15;Normal
-2:15;Normal
-1:15;Normal
0:15;Normal
1:15;Wall
2:15;Normal
3:15;Normal
4:15;Normal
5:15;Normal
6:15;Normal
7:15;Normal
8:15;Normal
9:15;Normal
10:15;Wall
11:15;Normal
12:15;Normal
13:15;Normal
14:15;Normal
15:15;Normal
16:15;Normal
17:15;Normal
18:15;Normal
19:15;Wall
20:15;Normal
21:15;Normal
22:15;Normal
23:15;Normal
24:15;Wall
25:15;Wall
26:15;Wall
27:15;Wall
28:15;Wall
-8:16;Wall
-7:16;Wall
-6:16;Wall
-5:16;Wall
-4:16;Normal
-3:16;Normal
-2:16;Normal
-1:16;Normal
0:16;Wall
1:16;Wall
2:16;Wall
3:16;Wall
4:16;Normal
5:16;Normal
6:16;Normal
7:16;Wall
8:16;Wall
9:16;Wall
10:16;Wall
11:16;Wall
12:16;Wall
13:16;Wall
14:16;Wall
15:16;Wall
16:16;Normal
17:16;Wall
18:16;Wall
19:16;Wall
20:16;Normal
21:16;Normal
22:16;Normal
23:16;Normal
24:16;Wall
25:16;Wall
26:16;Wall
27:16;Wall
-8:17;Wall
-7:17;Wall
-6:17;Wall
-5:17;Normal
-4:17;Normal
-3:17;Normal
-2:17;Normal
-1:17;Normal
0:17;Normal
1:17;Normal
2:17;Normal
3:17;Normal
4:17;Normal
5:17;Normal
6:17;Normal
7:17;Normal
8:17;Normal
9:17;Normal
10:17;Normal
11:17;Normal
12:17;Normal
13:17;Normal
14:17;Normal
15:17;Normal
16:17;Normal
17:17;Normal
18:17;Normal
19:17;Normal
20:17;Normal
21:17;Normal
22:17;Normal
23:17;Normal
24:17;Wall
25:17;Wall
26:17;Wall
27:17;Wall
-9:18;Wall
-8:18;SpawnPoint1
-7:18;SpawnPoint1
-6:18;Normal
-5:18;Normal
-4:18;Normal
-3:18;Normal
-2:18;Normal
-1:18;Normal
0:18;Normal
1:18;Normal
2:18;Normal
3:18;Normal
4:18;Normal
5:18;Normal
6:18;Normal
7:18;Normal
8:18;Normal
9:18;Normal
10:18;Normal
11:18;Normal
12:18;Normal
13:18;Normal
14:18;Normal
15:18;Normal
16:18;Normal
17:18;Normal
18:18;Normal
19:18;Normal
20:18;Normal
21:18;Normal
22:18;Normal
23:18;Normal
24:18;SpawnPoint2
25:18;SpawnPoint2
26:18;Wall
-9:19;Wall
-8:19;SpawnPoint1
-7:19;SpawnPoint1
-6:19;Normal
-5:19;Normal
-4:19;Normal
-3:19;Normal
-2:19;Normal
-1:19;Normal
0:19;Normal
1:19;Normal
2:19;Normal
3:19;Normal
4:19;Normal
5:19;Normal
6:19;Normal
7:19;Normal
8:19;Normal
9:19;Normal
10:19;Normal
11:19;Normal
12:19;Normal
13:19;Normal
14:19;Normal
15:19;Normal
16:19;Normal
17:19;Normal
18:19;Normal
19:19;Normal
20:19;Normal
21:19;Normal
22:19;Normal
23:19;SpawnPoint2
24:19;SpawnPoint2
25:19;Wall
26:19;Wall
-10:20;Wall
-9:20;SpawnPoint1
-8:20;SpawnPoint1
-7:20;Normal
-6:20;Normal
-5:20;Normal
-4:20;Normal
-3:20;Normal
-2:20;Normal
-1:20;Normal
0:20;Normal
1:20;Normal
2:20;Normal
3:20;Normal
4:20;Normal
5:20;Normal
6:20;Normal
7:20;Normal
8:20;Normal
9:20;Normal
10:20;Normal
11:20;Normal
12:20;Normal
13:20;Normal
14:20;Normal
15:20;Normal
16:20;Normal
17:20;Normal
18:20;Normal
19:20;Normal
20:20;Normal
21:20;Normal
22:20;Normal
23:20;SpawnPoint2
24:20;SpawnPoint2
25:20;Wall
-10:21;Wall
-9:21;Wall
-8:21;Wall
-7:21;Normal
-6:21;Normal
-5:21;Normal
-4:21;Normal
-3:21;Normal
-2:21;Normal
-1:21;Normal
0:21;Normal
1:21;Normal
2:21;Normal
3:21;Normal
4:21;Normal
5:21;Normal
6:21;Normal
7:21;Normal
8:21;Normal
9:21;Normal
10:21;Normal
11:21;Normal
12:21;Normal
13:21;Normal
14:21;Normal
15:21;Normal
16:21;Normal
17:21;Normal
18:21;Normal
19:21;Normal
20:21;Normal
21:21;Normal
22:21;Wall
23:21;Wall
24:21;Wall
25:21;Wall
-11:22;Wall
-10:22;Wall
-9:22;Wall
-8:22;Wall
-7:22;Normal
-6:22;Normal
-5:22;Normal
-4:22;Normal
-3:22;Wall
-2:22;Wall
-1:22;Wall
0:22;Normal
1:22;Wall
2:22;Wall
3:22;Wall
4:22;Wall
5:22;Wall
6:22;Wall
7:22;Wall
8:22;Wall
9:22;Wall
10:22;Normal
11:22;Normal
12:22;Normal
13:22;Wall
14:22;Wall
15:22;Wall
16:22;Wall
17:22;Normal
18:22;Normal
19:22;Normal
20:22;Normal
21:22;Wall
22:22;Wall
23:22;Wall
24:22;Wall
-11:23;Wall
-10:23;Wall
-9:23;Wall
-8:23;Wall
-7:23;Normal
-6:23;Normal
-5:23;Normal
-4:23;Normal
-3:23;Wall
-2:23;Normal
-1:23;Normal
0:23;Normal
1:23;Normal
2:23;Normal
3:23;Normal
4:23;Normal
5:23;Normal
6:23;Normal
7:23;Wall
8:23;Normal
9:23;Normal
10:23;Normal
11:23;Normal
12:23;Normal
13:23;Normal
14:23;Normal
15:23;Wall
16:23;Normal
17:23;Normal
18:23;Normal
19:23;Normal
20:23;Wall
21:23;Wall
22:23;Wall
23:23;Wall
24:23;Wall
-12:24;Wall
-11:24;Wall
-10:24;Wall
-9:24;Wall
-8:24;Wall
-7:24;Normal
-6:24;Normal
-5:24;Normal
-4:24;Normal
-3:24;Wall
-2:24;Normal
-1:24;Normal
0:24;Normal
1:24;Normal
2:24;Normal
3:24;Normal
4:24;Normal
5:24;Normal
6:24;Wall
7:24;Normal
8:24;Normal
9:24;Normal
10:24;Normal
11:24;Normal
12:24;Normal
13:24;Normal
14:24;Wall
15:24;Normal
16:24;Normal
17:24;Normal
18:24;Normal
19:24;Wall
20:24;Wall
21:24;Wall
22:24;Wall
23:24;Wall
-12:25;Wall
-11:25;Wall
-10:25;Wall
-9:25;Wall
-8:25;Wall
-7:25;Normal
-6:25;Normal
-5:25;Normal
-4:25;Normal
-3:25;Wall
-2:25;Normal
-1:25;Normal
0:25;Normal
1:25;Normal
2:25;Normal
3:25;Normal
4:25;Normal
5:25;Wall
6:25;Normal
7:25;Normal
8:25;Normal
9:25;Normal
10:25;Normal
11:25;Normal
12:25;Normal
13:25;Wall
14:25;Normal
15:25;Normal
16:25;Normal
17:25;Normal
18:25;Wall
19:25;Wall
20:25;Wall
21:25;Wall
22:25;Wall
23:25;Wall
-13:26;Wall
-12:26;Wall
-11:26;Wall
-10:26;Wall
-9:26;Wall
-8:26;Wall
-7:26;Normal
-6:26;Normal
-5:26;Normal
-4:26;Normal
-3:26;Normal
-2:26;Normal
-1:26;Normal
0:26;Normal
1:26;Normal
2:26;Normal
3:26;Normal
4:26;Normal
5:26;Normal
6:26;Normal
7:26;Normal
8:26;Normal
9:26;Normal
10:26;Normal
11:26;Normal
12:26;Wall
13:26;Normal
14:26;Normal
15:26;Normal
16:26;Normal
17:26;Wall
18:26;Wall
19:26;Wall
20:26;Wall
21:26;Wall
22:26;Wall
-13:27;Wall
-12:27;Wall
-11:27;Wall
-10:27;Wall
-9:27;Wall
-8:27;Wall
-7:27;Normal
-6:27;Normal
-5:27;Normal
-4:27;Normal
-3:27;Normal
-2:27;Normal
-1:27;Normal
0:27;Normal
1:27;Normal
2:27;Normal
3:27;Normal
4:27;Wall
5:27;Normal
6:27;Normal
7:27;Normal
8:27;Normal
9:27;Normal
10:27;Normal
11:27;Wall
12:27;Normal
13:27;Normal
14:27;Normal
15:27;Normal
16:27;Wall
17:27;Wall
18:27;Wall
19:27;Wall
20:27;Wall
21:27;Wall
22:27;Wall
-14:28;Wall
-13:28;Wall
-12:28;Wall
-11:28;Wall
-10:28;Wall
-9:28;Wall
-8:28;Wall
-7:28;Normal
-6:28;Normal
-5:28;Normal
-4:28;Normal
-3:28;Wall
-2:28;Normal
-1:28;Normal
0:28;Normal
1:28;Normal
2:28;Normal
3:28;Normal
4:28;Normal
5:28;Normal
6:28;Normal
7:28;Normal
8:28;Normal
9:28;Normal
10:28;Wall
11:28;Normal
12:28;Normal
13:28;Normal
14:28;Normal
15:28;Wall
16:28;Wall
17:28;Wall
18:28;Wall
19:28;Wall
20:28;Wall
21:28;Wall
-14:29;Wall
-13:29;Wall
-12:29;Wall
-11:29;Wall
-10:29;Wall
-9:29;Wall
-8:29;Wall
-7:29;Normal
-6:29;Normal
-5:29;Normal
-4:29;Normal
-3:29;Wall
-2:29;Normal
-1:29;Normal
0:29;Normal
1:29;Normal
2:29;Normal
3:29;Normal
4:29;Normal
5:29;Normal
6:29;Normal
7:29;Normal
8:29;Normal
9:29;Wall
10:29;Normal
11:29;Normal
12:29;Normal
13:29;Normal
14:29;Wall
15:29;Wall
16:29;Wall
17:29;Wall
18:29;Wall
19:29;Wall
20:29;Wall
21:29;Wall
-15:30;Wall
-14:30;Wall
-13:30;Wall
-12:30;Wall
-11:30;Wall
-10:30;Wall
-9:30;Wall
-8:30;Wall
-7:30;Normal
-6:30;Normal
-5:30;Normal
-4:30;Normal
-3:30;Normal
-2:30;Normal
-1:30;Normal
0:30;Normal
1:30;Normal
2:30;Normal
3:30;Normal
4:30;Normal
5:30;Normal
6:30;Normal
7:30;Normal
8:30;Wall
9:30;Normal
10:30;Normal
11:30;Normal
12:30;Normal
13:30;Wall
14:30;Wall
15:30;Wall
16:30;Wall
17:30;Wall
18:30;Wall
19:30;Wall
20:30;Wall
-15:31;Wall
-14:31;Wall
-13:31;Wall
-12:31;Wall
-11:31;Wall
-10:31;Wall
-9:31;Wall
-8:31;Wall
-7:31;Normal
-6:31;Normal
-5:31;Normal
-4:31;Normal
-3:31;Normal
-2:31;Wall
-1:31;Wall
0:31;Wall
1:31;Normal
2:31;Wall
3:31;Wall
4:31;Wall
5:31;Wall
6:31;Normal
7:31;Wall
8:31;Normal
9:31;Normal
10:31;Normal
11:31;Normal
12:31;Wall
13:31;Wall
14:31;Wall
15:31;Wall
16:31;Wall
17:31;Wall
18:31;Wall
19:31;Wall
20:31;Wall
-16:32;Wall
-15:32;Wall
-14:32;Wall
-13:32;Wall
-12:32;Wall
-11:32;Wall
-10:32;Wall
-9:32;Wall
-8:32;Wall
-7:32;Normal
-6:32;Normal
-5:32;Normal
-4:32;Normal
-3:32;Normal
-2:32;Normal
-1:32;Normal
0:32;Normal
1:32;Normal
2:32;Normal
3:32;Normal
4:32;Normal
5:32;Normal
6:32;Normal
7:32;Normal
8:32;Normal
9:32;Normal
10:32;Normal
11:32;Wall
12:32;Wall
13:32;Wall
14:32;Wall
15:32;Wall
16:32;Wall
17:32;Wall
18:32;Wall
19:32;Wall
-16:33;Wall
-15:33;Wall
-14:33;Wall
-13:33;Wall
-12:33;Wall
-11:33;Wall
-10:33;Wall
-9:33;Wall
-8:33;Wall
-7:33;Normal
-6:33;Normal
-5:33;Normal
-4:33;Normal
-3:33;Normal
-2:33;Normal
-1:33;Normal
0:33;Normal
1:33;Normal
2:33;Normal
3:33;Normal
4:33;Normal
5:33;Normal
6:33;Normal
7:33;Normal
8:33;Normal
9:33;Normal
10:33;Wall
11:33;Wall
12:33;Wall
13:33;Wall
14:33;Wall
15:33;Wall
16:33;Wall
17:33;Wall
18:33;Wall
19:33;Wall
-17:34;Wall
-16:34;Wall
-15:34;Wall
-14:34;Wall
-13:34;Wall
-12:34;Wall
-11:34;Wall
-10:34;Wall
-9:34;Wall
-8:34;Wall
-7:34;Normal
-6:34;Normal
-5:34;Normal
-4:34;Normal
-3:34;Normal
-2:34;Normal
-1:34;Normal
0:34;Normal
1:34;Normal
2:34;Normal
3:34;Normal
4:34;Normal
5:34;Normal
6:34;Normal
7:34;Normal
8:34;Normal
9:34;Wall
10:34;Wall
11:34;Wall
12:34;Wall
13:34;Wall
14:34;Wall
15:34;Wall
16:34;Wall
17:34;Wall
18:34;Wall
-17:35;Wall
-16:35;Wall
-15:35;Wall
-14:35;Wall
-13:35;Wall
-12:35;Wall
-11:35;Wall
-10:35;Wall
-9:35;Wall
-8:35;Wall
-7:35;Normal
-6:35;Normal
-5:35;Normal
-4:35;Normal
-3:35;Normal
-2:35;Normal
-1:35;Normal
0:35;Normal
1:35;Normal
2:35;Normal
3:35;Normal
4:35;Wall
5:35;Normal
6:35;Normal
7:35;Normal
8:35;Wall
9:35;Wall
10:35;Wall
11:35;Wall
12:35;Wall
13:35;Wall
14:35;Wall
15:35;Wall
16:35;Wall
17:35;Wall
18:35;Wall
-18:36;Wall
-17:36;Wall
-16:36;Wall
-15:36;Wall
-14:36;Wall
-13:36;Wall
-12:36;Wall
-11:36;Wall
-10:36;Wall
-9:36;Wall
-8:36;Wall
-7:36;Normal
-6:36;Normal
-5:36;Normal
-4:36;Normal
-3:36;Normal
-2:36;Normal
-1:36;Normal
0:36;Normal
1:36;Normal
2:36;Normal
3:36;Normal
4:36;Wall
5:36;Normal
6:36;Normal
7:36;Wall
8:36;Wall
9:36;Wall
10:36;Wall
11:36;Wall
12:36;Wall
13:36;Wall
14:36;Wall
15:36;Wall
16:36;Wall
17:36;Wall
-18:37;Wall
-17:37;Wall
-16:37;Wall
-15:37;Wall
-14:37;Wall
-13:37;Wall
-12:37;Wall
-11:37;Wall
-10:37;Wall
-9:37;Wall
-8:37;Wall
-7:37;Normal
-6:37;Normal
-5:37;Normal
-4:37;Normal
-3:37;Normal
-2:37;Normal
-1:37;Normal
0:37;Normal
1:37;Normal
2:37;Normal
3:37;Wall
4:37;Normal
5:37;Normal
6:37;Wall
7:37;Wall
8:37;Wall
9:37;Wall
10:37;Wall
11:37;Wall
12:37;Wall
13:37;Wall
14:37;Wall
15:37;Wall
16:37;Wall
17:37;Wall
-19:38;Wall
-18:38;Wall
-17:38;Wall
-16:38;Wall
-15:38;Wall
-14:38;Wall
-13:38;Wall
-12:38;Wall
-11:38;Wall
-10:38;Wall
-9:38;Wall
-8:38;Wall
-7:38;Wall
-6:38;Wall
-5:38;Wall
-4:38;Wall
-3:38;Wall
-2:38;Wall
-1:38;Wall
0:38;Wall
1:38;Wall
2:38;Wall
3:38;Wall
4:38;Wall
5:38;Wall
6:38;Wall
7:38;Wall
8:38;Wall
9:38;Wall
10:38;Wall
11:38;Wall
12:38;Wall
13:38;Wall
14:38;Wall
15:38;Wall
16:38;Wall

SpawnPoint1;SpawnPoint2")]
        public void SerializeAndDeserialize(string mapString)
        {
            HexMap map = HexMapSerializer.Deserialize(mapString);

            Assert.Equal(mapString, HexMapSerializer.Serialize(map));
        }
    }
}