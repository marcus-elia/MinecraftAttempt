using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HardCodedBuilding : MonoBehaviour
{
    public static string buildingFile = @"32,51,57
// ====================
//     Left arch
// ====================
// Left Pillar
0-1,0-12,0-7,limestone,t
// Right Pillar
10-11,0-12,0-7,limestone,t
// Arch
2,6-12,1,limestone,t
3,8-12,1,limestone,t
4,10-12,1,limestone,t
5-6,11-12,1,limestone,t
7,10-12,1,limestone,t
8,8-12,1,limestone,t
9,6-12,1,limestone,t
// Second layer of arch
2,5-12,2,limestone,t
3,7-12,2,limestone,t
4,9-12,2,limestone,t
5-6,10-12,2,limestone,t
7,9-12,2,limestone,t
8,7-12,2,limestone,t
9,5-12,2,limestone,t
// Third layer
2,0-12,3,limestone,t
3,5-12,3,limestone,t
4,8-12,3,limestone,t
5-6,9-12,3,limestone,t
7,8-12,3,limestone,t
8,5-12,3,limestone,t
9,0-12,3,limestone,t
// Fourth layer
2,0-12,4,limestone,t
3,3-12,4,limestone,t
4,6-12,4,limestone,t
5-6,7-12,4,limestone,t
7,6-12,4,limestone,t
8,3-12,4,limestone,t
9,0-12,4,limestone,t
// Fifth Layer
2,0-12,5,limestone,t
3,3-12,5,limestone,t
4,3-12,5,limestone,t
5-6,3-12,5,limestone,t
7,3-12,5,limestone,t
8,3-12,5,limestone,t
9,3-12,5,limestone,t
5-6,0-2,5,limestone,t
// ====================
//     Center arch
// ====================
// Left Pillar is shared with Right Pillar of Left arch
// Right Pillar
20-21,0-12,0-7,limestone,t
// Arch
12,6-12,1,limestone,t
13,8-12,1,limestone,t
14,10-12,1,limestone,t
15-16,11-12,1,limestone,t
17,10-12,1,limestone,t
18,8-12,1,limestone,t
19,6-12,1,limestone,t
// Second layer of arch
12,5-12,2,limestone,t
13,7-12,2,limestone,t
14,9-12,2,limestone,t
15-16,10-12,2,limestone,t
17,9-12,2,limestone,t
18,7-12,2,limestone,t
19,5-12,2,limestone,t
// Third layer
12,0-12,3,limestone,t
13,5-12,3,limestone,t
14,8-12,3,limestone,t
15-16,9-12,3,limestone,t
17,8-12,3,limestone,t
18,5-12,3,limestone,t
19,0-12,3,limestone,t
// Fourth layer
12,0-12,4,limestone,t
13,3-12,4,limestone,t
14,6-12,4,limestone,t
15-16,7-12,4,limestone,t
17,6-12,4,limestone,t
18,3-12,4,limestone,t
19,0-12,4,limestone,t
// Fifth Layer
12,0-12,5,limestone,t
13,3-12,5,limestone,t
14,3-12,5,limestone,t
15-16,3-12,5,limestone,t
17,3-12,5,limestone,t
18,3-12,5,limestone,t
19,3-12,5,limestone,t
15-16,0-2,5,limestone,t
// ====================
//     Right arch
// ====================
// Left Pillar is shared with Right Pillar of Center arch
// Right Pillar
30-31,0-12,0-7,limestone,t
// Arch
22,6-12,1,limestone,t
23,8-12,1,limestone,t
24,10-12,1,limestone,t
25-26,11-12,1,limestone,t
27,10-12,1,limestone,t
28,8-12,1,limestone,t
29,6-12,1,limestone,t
// Second layer of arch
22,5-12,2,limestone,t
23,7-12,2,limestone,t
24,9-12,2,limestone,t
25-26,10-12,2,limestone,t
27,9-12,2,limestone,t
28,7-12,2,limestone,t
29,5-12,2,limestone,t
// Third layer
22,0-12,3,limestone,t
23,5-12,3,limestone,t
24,8-12,3,limestone,t
25-26,9-12,3,limestone,t
27,8-12,3,limestone,t
28,5-12,3,limestone,t
29,0-12,3,limestone,t
// Fourth layer
22,0-12,4,limestone,t
23,3-12,4,limestone,t
24,6-12,4,limestone,t
25-26,7-12,4,limestone,t
27,6-12,4,limestone,t
28,3-12,4,limestone,t
29,0-12,4,limestone,t
// Fifth Layer
22,0-12,5,limestone,t
23,3-12,5,limestone,t
24,3-12,5,limestone,t
25-26,3-12,5,limestone,t
27,3-12,5,limestone,t
28,3-12,5,limestone,t
29,3-12,5,limestone,t
25-26,0-2,5,limestone,t
// ====================
//    Middle Section
// ====================
// Pillar 1
0-1,13-20,0-7,limestone,t
// Pillar 2
10-11,13-20,0-7,limestone,t
// Pillar 3
20-21,13-20,0-7,limestone,t
// Pillar 4
30-31,13-20,0-7,limestone,t
// Fence
0-31,13,0,limestonefence,t
// Left windows
2,13-20,1,limestone,t
3-4,13-17,2,darkglass,t
3-4,18-20,1,limestone,t
5-6,13-20,1,limestone,t
7-8,13-17,2,darkglass,t
7-8,18-20,1,limestone,t
9,13-20,1,limestone,t
// Right windows
22,13-20,1,limestone,t
23-24,13-17,2,darkglass,t
23-24,18-20,1,limestone,t
25-26,13-20,1,limestone,t
27-28,13-17,2,darkglass,t
27-28,18-20,1,limestone,t
29,13-20,1,limestone,t
// Center circle
12,13-15,1,limestone,t
12,16-17,2,darkglass,t
12,18-20,1,limestone,t
13,13-14,1,limestone,t
13,15-18,2,darkglass,t
13,19-20,1,limestone,t
14,13,1,limestone,t
14,14-19,2,darkglass,t
14,20,1,limestone,t
15-16,13-20,2,darkglass,t
17,13,1,limestone,t
17,14-19,2,darkglass,t
17,20,1,limestone,t
18,13-14,1,limestone,t
18,15-18,2,darkglass,t
18,19-20,1,limestone,t
19,13-15,1,limestone,t
19,16-17,2,darkglass,t
19,18-20,1,limestone,t
// Floors
1-29,12,1-7,limestone,t
// ====================
//      Layer 3
// ====================
// Pillars
0-1,21-26,0-7,limestone,t
10-11,21-26,0-1,limestone,t
10-11,21-26,6-7,limestone,t
20-21,21-26,0-1,limestone,t
20-21,21-26,6-7,limestone,t
30-31,21-26,0-7,limestone,t
// Center
13,21-26,1,limestone,t
15-16,21-26,1,limestone,t
18,21-26,1,limestone,t
// Left
3,21-26,1,limestone,t
5-6,21-26,1,limestone,t
8,21-26,1,limestone,t
2-9,21-26,3,limestone,t
// Right
23,21-26,1,limestone,t
25-26,21-26,1,limestone,t
28,21-26,1,limestone,t
22-29,21-26,3,limestone,t
// Floors
2-29,21,1-7,limestone,t
// Fence
0-30,22,0,limestonefence,t
// =======================
//       Bell Towers
// =======================
// Fence
0-1,27,0,limestonefence,t
2-9,27,1,limestonefence,t
10-11,27,0,limestonefence,t
12-19,27,1,limestonefence,t
20-21,27,0,limestonefence,t
22-29,27,1,limestonefence,t
30-31,27,0,limestonefence,t
// --- Left Tower ----
// Pillars
0,27-38,1-2,limestone,t
1,27-38,1,limestone,t
5-6,28-38,1,limestone,t
10,27-38,1,limestone,t
11,27-38,1-2,limestone,t
0,27-38,6-7,limestone,t
1,27-38,7,limestone,t
5-6,27-38,7,limestone,t
10,27-38,7,limestone,t
11,27-38,6-7,limestone,t
// Top of arches
//    Left Front
2,37-38,2,limestone,t
3,38,2,limestone,t
4,37-38,2,limestone,t
//    Right Front
7,37-38,2,limestone,t
8,38,2,limestone,t
9,37-38,2,limestone,t
//    Front Top
0-11,39,1,limestone,t
//    Left Back
2,37-38,6,limestone,t
3,38,6,limestone,t
4,37-38,6,limestone,t
//    Right Back
7,37-38,6,limestone,t
8,38,6,limestone,t
9,37-38,6,limestone,t
//    Back Top
0-11,39,7,limestone,t
//    Left
1,37-38,3,limestone,t
1,38,4,limestone,t
1,37-38,5,limestone,t
//    Left Top
0,39,2-6,limestone,t
//    Right
10,37-38,3,limestone,t
10,38,4,limestone,t
10,37-38,5,limestone,t
//    Right Top
11,39,2-6,limestone,t
// Darkness in the bells
2-9,27-37,3,darkglass,t
2-9,27-37,5,darkglass,t
2,27-37,4,darkglass,t
9,27-37,4,darkglass,t
// Floors
1-11,26,2,limestone,t
1-10,26,6,limestone,t
1,26,3-5,limestone,t
10-11,26,3-5,limestone,t
// Roof
1-10,39,2-6,limestone,t
// Fence
0-11,40,1,limestonefence,t
0-11,40,7,limestonefence,t
0,40,1-6,limestonefence,t
11,40,1-6,limestonefence,t
// --- Right Tower ----
// Pillars
20,27-38,1-2,limestone,t
21,27-38,1,limestone,t
25-26,28-38,1,limestone,t
30,27-38,1,limestone,t
31,27-38,1-2,limestone,t
20,27-38,6-7,limestone,t
21,27-38,7,limestone,t
25-26,27-38,7,limestone,t
30,27-38,7,limestone,t
31,27-38,6-7,limestone,t
// Top of arches
//    Left Front
22,37-38,2,limestone,t
23,38,2,limestone,t
24,37-38,2,limestone,t
//    Right Front
27,37-38,2,limestone,t
28,38,2,limestone,t
29,37-38,2,limestone,t
//    Front Top
20-31,39,1,limestone,t
//    Left Back
22,37-38,6,limestone,t
23,38,6,limestone,t
24,37-38,6,limestone,t
//    Right Back
27,37-38,6,limestone,t
28,38,6,limestone,t
29,37-38,6,limestone,t
//    Back Top
20-31,39,7,limestone,t
//    Left
21,37-38,3,limestone,t
21,38,4,limestone,t
21,37-38,5,limestone,t
//    Left Top
20,39,2-6,limestone,t
//    Right
30,37-38,3,limestone,t
30,38,4,limestone,t
30,37-38,5,limestone,t
//    Right Top
31,39,2-6,limestone,t
// Darkness in the bells
22-29,27-37,3,darkglass,t
22-29,27-37,5,darkglass,t
22,27-37,4,darkglass,t
29,27-37,4,darkglass,t
// Floors
21-30,26,2,limestone,t
21-30,26,6,limestone,t
20-21,26,3-5,limestone,t
30,26,3-5,limestone,t
// Roof
21-30,39,2-6,limestone,t
// Fence
20-31,40,1,limestonefence,t
20-31,40,7,limestonefence,t
20,40,1-6,limestonefence,t
31,40,1-6,limestonefence,t
// =========================
//  Between the bell towers
// =========================
12-19,26,1-7,limestone,t
12-19,27,7,limestonefence,t
// ===========================
//   Roof Middle of Building
// ===========================
10,20,8-24,stone,t
11,21,8-25,stone,t
12,22,8-26,stone,t
13,23,8-27,stone,t
14,24,8-28,stone,t
15,25,8-29,stone,t
16,25,8-29,stone,t
17,24,8-28,stone,t
18,23,8-27,stone,t
19,22,8-26,stone,t
20,21,8-25,stone,t
21,20,8-24,stone,t
// ===========================
//         Roof Wings
// ===========================
// Right side
21-31,20,24,stone,t
20-31,21,25,stone,t
19-31,22,26,stone,t
18-31,23,27,stone,t
17-31,24,28,stone,t
16-31,25,29,stone,t
16-31,25,30,stone,t
17-31,24,31,stone,t
18-31,23,32,stone,t
19-31,22,33,stone,t
20-31,21,34,stone,t
21-31,20,35,stone,t
// Left side
0-10,20,24,stone,t
0-11,21,25,stone,t
0-12,22,26,stone,t
0-13,23,27,stone,t
0-14,24,28,stone,t
0-15,25,29,stone,t
0-15,25,30,stone,t
0-14,24,31,stone,t
0-13,23,32,stone,t
0-12,22,33,stone,t
0-11,21,34,stone,t
0-10,20,35,stone,t
// =================
//     Far Roof
// =================
10,20,36-44,stone,t
11,21,35-44,stone,t
12,22,34-44,stone,t
13,23,33-44,stone,t
14,24,32-44,stone,t
15,25,31-44,stone,t
16,25,31-44,stone,t
17,24,32-44,stone,t
18,23,33-44,stone,t
19,22,34-44,stone,t
20,21,35-44,stone,t
21,20,36-44,stone,t
// ================
//    End Cap
// ================
// Bottom
11,20,45-46,stone,t
12,20,47,stone,t
13,20,48,stone,t
14-17,20,49,stone,t
18,20,48,stone,t
19,20,47,stone,t
20,20,45-46,stone,t
// Layer 1
12,21,45-46,stone,t
13,21,47,stone,t
14,21,48,stone,t
15-16,21,49,stone,t
17,21,48,stone,t
18,21,47,stone,t
19,21,45-46,stone,t
// Layer 2
13,22,45-46,stone,t
14,22,47,stone,t
15-16,22,48,stone,t
17,22,47,stone,t
18,22,45-46,stone,t
// Layer 4
13,23,45,stone,t
14,23,46,stone,t
15-16,23,47,stone,t
17,23,46,stone,t
18,23,45,stone,t
// Layer 5
14,24,45,stone,t
15-16,24,46,stone,t
17,24,45,stone,t
// Layer 6
15-16,25,45,stone,t
// ===========================
//    Middle Roof Perimeter
// ===========================
22-24,14,8-23,stone,t
7-9,14,8-23,stone,t
22-24,14,35-45,stone,t
7-9,14,35-45,stone,t
// End semicircle
8-10,14,45-46,stone,t
8-11,14,47,stone,t
9-12,14,48,stone,t
10-13,14,49,stone,t
11-20,14,50,stone,t
12-19,14,51,stone,t
13-18,14,52,stone,t
18-21,14,49,stone,t
19-22,14,48,stone,t
20-23,14,47,stone,t
21-24,14,45-46,stone,t
// ==========================
//    Outer Roof Perimeter
// ==========================
25-31,8,8-23,stone,t
0-6,8,8-23,stone,t
25-31,8,35-45,stone,t
0-6,8,35-45,stone,t
// End semicircle
0-6,8,45-46,stone,t
0-7,8,47,stone,t
1-8,8,48,stone,t
2-9,8,49,stone,t
3-10,8,50,stone,t
4-11,8,51,stone,t
5-12,8,52,stone,t
7-24,8,53,stone,t
9-22,8,54,stone,t
11-20,8,55,stone,t
14-17,8,56,stone,t
19-26,8,52,stone,t
20-27,8,51,stone,t
21-28,8,50,stone,t
22-29,8,49,stone,t
23-30,8,48,stone,t
24-31,8,47,stone,t
25-31,8,45-46,stone,t
// ===================
//    Upper Windows
// ===================
// ---- Right side ----
// Window sills
21,14,8-24,limestone,t
// Outline
21,15-19,8,limestone,t
21,18-19,9,limestone,t
21,19,10,limestone,t
21,18-19,11,limestone,t
21,15-19,12,limestone,t
// Window
20,15-19,9-11,darkglass,t
// Outline
21,18-19,13,limestone,t
21,19,14,limestone,t
21,18-19,15,limestone,t
21,15-19,16,limestone,t
// Window
20,15-19,13-15,darkglass,t
// Outline
21,18-19,17,limestone,t
21,19,18,limestone,t
21,18-19,19,limestone,t
21,15-19,20,limestone,t
// Window
20,15-19,17-19,darkglass,t
// Outline
21,18-19,21,limestone,t
21,19,22,limestone,t
21,18-19,23,limestone,t
21,15-19,24,limestone,t
// Window
20,15-19,21-23,darkglass,t
// Window sills
21,14,35-42,limestone,t
// Outline
21,15-19,35,limestone,t
21,18-19,36,limestone,t
21,19,37,limestone,t
21,18-19,38,limestone,t
21,15-19,39,limestone,t
// Window
20,15-19,36-38,darkglass,t
// Outline
21,18-19,40,limestone,t
21,19,41,limestone,t
21,18-19,42,limestone,t
21,15-19,43-44,limestone,t
// Window
20,15-19,40-42,darkglass,t
// ---- Left side ----
// Window sills
10,14,8-24,limestone,t
// Outline
10,15-19,8,limestone,t
10,18-19,9,limestone,t
10,19,10,limestone,t
10,18-19,11,limestone,t
10,15-19,12,limestone,t
// Window
11,15-19,9-11,darkglass,t
// Outline
10,18-19,13,limestone,t
10,19,14,limestone,t
10,18-19,15,limestone,t
10,15-19,16,limestone,t
// Window
11,15-19,13-15,darkglass,t
// Outline
10,18-19,17,limestone,t
10,19,18,limestone,t
10,18-19,19,limestone,t
10,15-19,20,limestone,t
// Window
11,15-19,17-19,darkglass,t
// Outline
10,18-19,21,limestone,t
10,19,22,limestone,t
10,18-19,23,limestone,t
10,15-19,24,limestone,t
// Window
11,15-19,21-23,darkglass,t
// Window sills
10,14,35-42,limestone,t
// Outline
10,15-19,35,limestone,t
10,18-19,36,limestone,t
10,19,37,limestone,t
10,18-19,38,limestone,t
10,15-19,39,limestone,t
// Window
11,15-19,36-38,darkglass,t
// Outline
10,18-19,40,limestone,t
10,19,41,limestone,t
10,18-19,42,limestone,t
10,15-19,43-44,limestone,t
// Window
11,15-19,40-42,darkglass,t
// ------ End cap --------
11,15-19,45-46,limestone,t
12,15-19,47,limestone,t
13,15-19,48,limestone,t
18,15-19,48,limestone,t
19,15-19,47,limestone,t
20,15-19,45-46,limestone,t
// Window outline
14,15-19,49,limestone,t
17,15-19,49,limestone,t
15-16,19,49,limestone,t
15-16,14,49,limestone,t
// Window
15-16,15-18,48,darkglass,t
// ===========================
//   Middle Layer of Windows
// ===========================
// ---- Right side ------
// Outline
24,9-13,8,limestone,t
24,9-10,9,limestone,t
24,12-13,9,limestone,t
24,9,10,limestone,t
24,13,10,limestone,t
24,9-10,11,limestone,t
24,12-13,11,limestone,t
24,9-13,12,limestone,t
// Window
23,10-12,9-11,darkglass,t
// Outline
24,9-10,13,limestone,t
24,12-13,13,limestone,t
24,9,14,limestone,t
24,13,14,limestone,t
24,9-10,15,limestone,t
24,12-13,15,limestone,t
24,9-13,16,limestone,t
// Window
23,10-12,13-15,darkglass,t
// Outline
24,9-10,13,limestone,t
24,12-13,13,limestone,t
24,9,14,limestone,t
24,13,14,limestone,t
24,9-10,15,limestone,t
24,12-13,15,limestone,t
24,9-13,16,limestone,t
// Window
23,10-12,13-15,darkglass,t
// Outline
24,9-10,17,limestone,t
24,12-13,17,limestone,t
24,9,18,limestone,t
24,13,18,limestone,t
24,9-10,19,limestone,t
24,12-13,19,limestone,t
24,9-13,20,limestone,t
// Window
23,10-12,17-19,darkglass,t
// Outline
24,9-10,21,limestone,t
24,12-13,21,limestone,t
24,9,22,limestone,t
24,13,22,limestone,t
24,9-10,23,limestone,t
24,12-13,23,limestone,t
24,9-13,24,limestone,t
// Window
23,10-12,21-23,darkglass,t
// Outline
24,9-13,35,limestone,t
24,9-10,36,limestone,t
24,12-13,36,limestone,t
24,9,37,limestone,t
24,13,37,limestone,t
24,9-10,38,limestone,t
24,12-13,38,limestone,t
24,9-13,39,limestone,t
// Window
23,10-12,36-38,darkglass,t
// Outline
24,9-10,40,limestone,t
24,12-13,40,limestone,t
24,9,41,limestone,t
24,13,41,limestone,t
24,9-10,42,limestone,t
24,12-13,42,limestone,t
24,9-13,43,limestone,t
24,9-13,44,limestone,t
// Window
23,10-12,40-42,darkglass,t
// ---- Left side ----
// Outline
7,9-13,8,limestone,t
7,9-10,9,limestone,t
7,12-13,9,limestone,t
7,9,10,limestone,t
7,13,10,limestone,t
7,9-10,11,limestone,t
7,12-13,11,limestone,t
7,9-13,12,limestone,t
// Window
8,10-12,9-11,darkglass,t
// Outline
7,9-10,13,limestone,t
7,12-13,13,limestone,t
7,9,14,limestone,t
7,13,14,limestone,t
7,9-10,15,limestone,t
7,12-13,15,limestone,t
7,9-13,16,limestone,t
// Window
8,10-12,13-15,darkglass,t
// Outline
7,9-10,13,limestone,t
7,12-13,13,limestone,t
7,9,14,limestone,t
7,13,14,limestone,t
7,9-10,15,limestone,t
7,12-13,15,limestone,t
7,9-13,16,limestone,t
// Window
8,10-12,13-15,darkglass,t
// Outline
7,9-10,17,limestone,t
7,12-13,17,limestone,t
7,9,18,limestone,t
7,13,18,limestone,t
7,9-10,19,limestone,t
7,12-13,19,limestone,t
7,9-13,20,limestone,t
// Window
8,10-12,17-19,darkglass,t
// Outline
7,9-10,21,limestone,t
7,12-13,21,limestone,t
7,9,22,limestone,t
7,13,22,limestone,t
7,9-10,23,limestone,t
7,12-13,23,limestone,t
7,9-13,24,limestone,t
// Window
8,10-12,21-23,darkglass,t
// Outline
7,9-13,35,limestone,t
7,9-10,36,limestone,t
7,12-13,36,limestone,t
7,9,37,limestone,t
7,13,37,limestone,t
7,9-10,38,limestone,t
7,12-13,38,limestone,t
7,9-13,39,limestone,t
// Window
8,10-12,36-38,darkglass,t
// Outline
7,9-10,40,limestone,t
7,12-13,40,limestone,t
7,9,41,limestone,t
7,13,41,limestone,t
7,9-10,42,limestone,t
7,12-13,42,limestone,t
7,9-13,43,limestone,t
7,9-13,44,limestone,t
// Window
8,10-12,40-42,darkglass,t
// ---- End cap ----
7,9-13,45-46,limestone,t
8,9-13,45-47,limestone,t
9,9-13,48,limestone,t
10,9-13,49,limestone,t
11,9-13,50,limestone,t
12,9-13,51,limestone,t
13-18,9-13,52,limestone,t
19,9-13,51,limestone,t
20,9-13,50,limestone,t
21,9-13,49,limestone,t
22,9-13,48,limestone,t
23,9-13,47,limestone,t
24,9-13,45-46,limestone,t
// ==========================
//  Bottom Layer of Windows
// ==========================
// ---- Right side ----
// Outline
31,0-7,8,limestone,t
31,0,9-11,limestone,t
31,7,9-11,limestone,t
31,0-7,12,limestone,t
// Window
30,1-6,9-11,darkglass,t
// Outline
31,0,13-15,limestone,t
31,7,13-15,limestone,t
31,0-7,16,limestone,t
// Window
30,1-6,13-15,darkglass,t
// Outline
31,0,17-19,limestone,t
31,7,17-19,limestone,t
31,0-7,20,limestone,t
// Window
30,1-6,17-19,darkglass,t
// Outline
31,0,21-23,limestone,t
31,7,21-23,limestone,t
31,0-7,24,limestone,t
// Window
30,1-6,21-23,darkglass,t
// Outline
31,0-7,35,limestone,t
31,0,36-38,limestone,t
31,7,36-38,limestone,t
31,0-7,39,limestone,t
// Window
30,1-6,36-38,darkglass,t
// Outline
31,0,40-42,limestone,t
31,7,40-42,limestone,t
31,0-7,43-44,limestone,t
// Window
30,1-6,40-42,darkglass,t
// ---- Left side ----
// Outline
0,0-7,8,limestone,t
0,0,9-11,limestone,t
0,7,9-11,limestone,t
0,0-7,12,limestone,t
// Window
1,1-6,9-11,darkglass,t
// Outline
0,0,13-15,limestone,t
0,7,13-15,limestone,t
0,0-7,16,limestone,t
// Window
1,1-6,13-15,darkglass,t
// Outline
0,0,17-19,limestone,t
0,7,17-19,limestone,t
0,0-7,20,limestone,t
// Window
1,1-6,17-19,darkglass,t
// Outline
0,0,21-23,limestone,t
0,7,21-23,limestone,t
0,0-7,24,limestone,t
// Window
1,1-6,21-23,darkglass,t
// Outline
0,0-7,35,limestone,t
0,0,36-38,limestone,t
0,7,36-38,limestone,t
0,0-7,39,limestone,t
// Window
1,1-6,36-38,darkglass,t
// Outline
0,0,40-42,limestone,t
0,7,40-42,limestone,t
0,0-7,43-44,limestone,t
// Window
1,1-6,40-42,darkglass,t
// ---- End cap ----
0,0-7,45-47,limestone,t
1,0-7,48,limestone,t
2,0-7,49,limestone,t
3,0-7,50,limestone,t
4,0-7,51,limestone,t
5-6,0-7,52,limestone,t
7-8,0-7,53,limestone,t
9-10,0-7,54,limestone,t
11-13,0-7,55,limestone,t
14-17,0-7,56,limestone,t
18-20,0-7,55,limestone,t
21-22,0-7,54,limestone,t
23-24,0-7,53,limestone,t
25-26,0-7,52,limestone,t
26,0-7,51,limestone,t
27,0-7,50,limestone,t
28,0-7,49,limestone,t
29,0-7,48,limestone,t
30,0-7,45-47,limestone,t
// ========================
//     Wing Walls
// ========================
// ---- Right side ----
21-31,0-19,24,limestone,t
21-31,0-19,35,limestone,t
// Facade
31,24,29-30,limestone,t
31,23,28-31,limestone,t
31,22,27-32,limestone,t
31,21,26-33,limestone,t
31,20,25-34,limestone,t
31,19,24-35,limestone,t
// Glass circle
30,18,29-30,darkglass,t
30,17,27-32,darkglass,t
30,16,26-33,darkglass,t
30,14-15,25-34,darkglass,t
30,13,26-33,darkglass,t
30,12,27-32,darkglass,t
30,11,29-30,darkglass,t
// Outline of glass circle
31,18,25-28,limestone,t
31,18,31-34,limestone,t
31,17,25-26,limestone,t
31,17,33-34,limestone,t
31,16,25,limestone,t
31,16,34,limestone,t
31,13,25,limestone,t
31,13,34,limestone,t
31,12,25-26,limestone,t
31,12,33-34,limestone,t
31,11,25-28,limestone,t
31,11,31-34,limestone,t
// Pillars
31,10,25-34,limestone,t
31,6-9,26,limestone,t
31,6-9,28,limestone,t
31,6-9,31,limestone,t
31,6-9,33,limestone,t
31,4-5,25-34,limestone,t
// Doors
31,0-3,25,limestone,t
31,0-3,29-30,limestone,t
31,0-3,34,limestone,t
// ---- Left side ----
0-10,0-19,24,limestone,t
0-10,0-19,35,limestone,t
// Facade
0,24,29-30,limestone,t
0,23,28-31,limestone,t
0,22,27-32,limestone,t
0,21,26-33,limestone,t
0,20,25-34,limestone,t
0,19,24-35,limestone,t
// Glass circle
1,18,29-30,darkglass,t
1,17,27-32,darkglass,t
1,16,26-33,darkglass,t
1,14-15,25-34,darkglass,t
1,13,26-33,darkglass,t
1,12,27-32,darkglass,t
1,11,29-30,darkglass,t
// Outline of glass circle
0,18,25-28,limestone,t
0,18,31-34,limestone,t
0,17,25-26,limestone,t
0,17,33-34,limestone,t
0,16,25,limestone,t
0,16,34,limestone,t
0,13,25,limestone,t
0,13,34,limestone,t
0,12,25-26,limestone,t
0,12,33-34,limestone,t
0,11,25-28,limestone,t
0,11,31-34,limestone,t
// Pillars
0,10,25-34,limestone,t
0,6-9,26,limestone,t
0,6-9,28,limestone,t
0,6-9,31,limestone,t
0,6-9,33,limestone,t
0,4-5,25-34,limestone,t
// Doors
0,0-3,25,limestone,t
0,0-3,29-30,limestone,t
0,0-3,34,limestone,t
// Spire
15-16,25-35,28,stone,t
17,24-35,29,stone,t
18,25-35,30-31,stone,t
17,24-35,32,stone,t
15-16,25-35,33,stone,t
14,24-35,32,stone,t
13,25-35,30-31,stone,t
14,24-35,29,stone,t
15-16,36-42,29,stone,t
17,36-42,30-31,stone,t
15-16,36-42,32,stone,t
14,36-42,30-31,stone,t
15-16,43-50,30-31,stone,t";
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
