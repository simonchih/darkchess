import random, os
import math
import time 
import pygame
import copy
from pygame.locals import *
from sys import exit
from chess import *

background_image_filename = 'Image/SHEET.gif'
image_chess_back = 'Image/back.gif'
image_chess_bk = 'Image/BK.gif'
image_chess_ba = 'Image/BA.gif'
image_chess_bb = 'Image/BB.gif'
image_chess_br = 'Image/BR.gif'
image_chess_bn = 'Image/BN.gif'
image_chess_bc = 'Image/BC.gif'
image_chess_bp = 'Image/BP.gif'
image_chess_rk = 'Image/RK.gif'
image_chess_ra = 'Image/RA.gif'
image_chess_rb = 'Image/RB.gif'
image_chess_rr = 'Image/RR.gif'
image_chess_rn = 'Image/RN.gif'
image_chess_rc = 'Image/RC.gif'
image_chess_rp = 'Image/RP.gif'

s_newgame = 'Sound/NEWGAME.WAV'
s_capture = 'Sound/CAPTURE2.WAV'
s_click   = 'Sound/CLICK.WAV'
s_loss    = 'Sound/LOSS.WAV'
s_move2   = 'Sound/MOVE2.WAV'
s_win     = 'Sound/WIN.WAV'

SCREEN_SIZE = (521, 313) 
pygame.init()

screen = pygame.display.set_mode(SCREEN_SIZE, 0, 32)#SCREEN_SIZE, FULLSCREEN, 32)
pygame.display.set_caption("Taiwan Blind Chess")

background = pygame.image.load(background_image_filename).convert()
chess_back = pygame.image.load(image_chess_back).convert()
chess_bk = pygame.image.load(image_chess_bk).convert()
chess_ba = pygame.image.load(image_chess_ba).convert()
chess_bb = pygame.image.load(image_chess_bb).convert()
chess_br = pygame.image.load(image_chess_br).convert()
chess_bn = pygame.image.load(image_chess_bn).convert()
chess_bc = pygame.image.load(image_chess_bc).convert()
chess_bp = pygame.image.load(image_chess_bp).convert()
chess_rk = pygame.image.load(image_chess_rk).convert()
chess_ra = pygame.image.load(image_chess_ra).convert()
chess_rb = pygame.image.load(image_chess_rb).convert()
chess_rr = pygame.image.load(image_chess_rr).convert()
chess_rn = pygame.image.load(image_chess_rn).convert()
chess_rc = pygame.image.load(image_chess_rc).convert()
chess_rp = pygame.image.load(image_chess_rp).convert()

sound_new     = pygame.mixer.Sound(s_newgame)
sound_capture = pygame.mixer.Sound(s_capture)
sound_click   = pygame.mixer.Sound(s_click)
sound_loss    = pygame.mixer.Sound(s_loss)
sound_win     = pygame.mixer.Sound(s_win)
sound_move    = pygame.mixer.Sound(s_move2)

cstart_x = 34
cstart_y = 51
cstart_x2 = 260
cstart_y2 = 51

player_win = 0
player_first = 0
first = 1
turn_id = 0
player_color = 0
com_color = 1
max_value = 0

top_number = 10
deep_level = 1

top_chess = []
top_map = []
chtemp = chess(0, 0, 0, (0, 0), (0, 0), chess_back.get_size(), chess_back)
my_ch = [[chtemp, chtemp, chtemp, chtemp, chtemp, chtemp, chtemp, chtemp], [chtemp, chtemp, chtemp, chtemp, chtemp, chtemp, chtemp, chtemp], [chtemp, chtemp, chtemp, chtemp, chtemp, chtemp, chtemp, chtemp], [chtemp, chtemp, chtemp, chtemp, chtemp, chtemp, chtemp, chtemp]]
chess_index = [0] * 32
map = [[(0,0)]*8, [(0,0)]*8, [(0,0)]*8, [(0,0)]*8]
cor = [[(0,0)]*8, [(0,0)]*8, [(0,0)]*8, [(0,0)]*8]
mark = [[0]*8, [0]*8, [0]*8, [0]*8]
king_live = [1, 1]
chess_num = [16, 16]

def index_to_chess_surface(index):
    if 0 <= index < 5:
        return chess_bp
    elif index < 7:
        return chess_bc
    elif index < 9:
        return chess_bn
    elif index < 11:
        return chess_br
    elif index < 13:
        return chess_bb
    elif index < 15:
        return chess_ba
    elif 15 == index:
        return chess_bk
    elif 16 <= index < 21:
        return chess_rp
    elif index < 23:
        return chess_rc
    elif index < 25:
        return chess_rn
    elif index < 27:
        return chess_rr
    elif index < 29:
        return chess_rb
    elif index < 31:
        return chess_ra
    elif 31 == index:
        return chess_rk

def index_to_chess_value(index):
    if 0 <= index < 5:
        return 1
    elif index < 7:
        return 2
    elif index < 9:
        return 3
    elif index < 11:
        return 4
    elif index < 13:
        return 5
    elif index < 15:
        return 6
    elif 15 == index:
        return 7
    elif 16 <= index < 21:
        return 1
    elif index < 23:
        return 2
    elif index < 25:
        return 3
    elif index < 27:
        return 4
    elif index < 29:
        return 5
    elif index < 31:
        return 6
    elif 31 == index:
        return 7

def ini_random_chess(list):
    all_list = [0] * 32
    for end in range(31, -1, -1):
        start = random.randint(0, end)
        i = start
        while i != -1 :
            if 0 == all_list[start]:
                if i != 0:
                    start += 1
                    start %= 32
                    i -= 1
                else:
                    break
            else:
                start += 1
                start %= 32
        all_list[start] = 1
        list[31-end] = start
    return list

def index_to_color(index):
    if 0 <= index < 16:
        return 0
    else:
        return 1

def findC(ch, x, y):
    for pr in ch:
        for p in pr:
            if math.hypot(p.x-x, p.y-y) <= p.size:
                return p
    return None

def all_chess_move(map, my_chess):
    for rowm in map:
        for m in rowm:
            if m!= (-1, -1) and 0 == my_chess[m[0]][m[1]].back:
                my_chess[m[0]][m[1]].possible_move, map, my_chess= collect_possible_move(my_chess[m[0]][m[1]].row, my_chess[m[0]][m[1]].col, map, my_chess)
    return map, my_chess
    
def collect_possible_move(i, j, map, my_ch):
    
    pm = []
    ncor = near(i,j)
    for nc in ncor:
        if (-1, -1) == map[nc[0]][nc[1]]:
            pm.append(nc)
        elif my_ch[map[nc[0]][nc[1]][0]][map[nc[0]][nc[1]][1]].color != my_ch[map[i][j][0]][map[i][j][1]].color and 0 == my_ch[map[nc[0]][nc[1]][0]][map[nc[0]][nc[1]][1]].back:
            if 1 == my_ch[map[i][j][0]][map[i][j][1]].value and 7 == my_ch[map[nc[0]][nc[1]][0]][map[nc[0]][nc[1]][1]].value:
                pm.append(nc)
            elif 7 == my_ch[map[i][j][0]][map[i][j][1]].value and 1 == my_ch[map[nc[0]][nc[1]][0]][map[nc[0]][nc[1]][1]].value:
                pass
            elif my_ch[map[i][j][0]][map[i][j][1]].value != 2 and my_ch[map[i][j][0]][map[i][j][1]].value >= my_ch[map[nc[0]][nc[1]][0]][map[nc[0]][nc[1]][1]].value:
                pm.append(nc)
    if 2 == my_ch[map[i][j][0]][map[i][j][1]].value:
        jump = 0
        for ii in range(i-1, -1, -1):
            if 1 == jump:
                if map[ii][j] == (-1, -1):
                    pass
                elif 1 == my_ch[map[ii][j][0]][map[ii][j][1]].back or my_ch[map[ii][j][0]][map[ii][j][1]].color == my_ch[map[i][j][0]][map[i][j][1]].color:
                    break
                else:
                    pm.append((ii, j))
                    break
            if map[ii][j] != (-1, -1):
                jump = 1
        jump = 0
        for ii in range(i+1, 4, 1):
            if 1 == jump:
                if map[ii][j] == (-1, -1):
                    pass
                elif 1 == my_ch[map[ii][j][0]][map[ii][j][1]].back or my_ch[map[ii][j][0]][map[ii][j][1]].color == my_ch[map[i][j][0]][map[i][j][1]].color:
                    break
                else:
                    pm.append((ii, j))
                    break
            if map[ii][j] != (-1, -1):
                jump = 1
        jump = 0
        for jj in range(j-1, -1, -1):
            if 1 == jump:
                if map[i][jj] == (-1, -1):
                    pass
                elif 1 == my_ch[map[i][jj][0]][map[i][jj][1]].back or my_ch[map[i][jj][0]][map[i][jj][1]].color == my_ch[map[i][j][0]][map[i][j][1]].color:
                    break
                else:
                    pm.append((i, jj))
                    break
            if map[i][jj] != (-1, -1):
                jump = 1
        jump = 0
        for jj in range(j+1, 8, 1):
            if map[i][jj] == (-1, -1):
                    pass
            elif 1 == jump:
                if 1 == my_ch[map[i][jj][0]][map[i][jj][1]].back or my_ch[map[i][jj][0]][map[i][jj][1]].color == my_ch[map[i][j][0]][map[i][j][1]].color:
                    break
                else:
                    pm.append((i, jj))
                    break
            if map[i][jj] != (-1, -1):
                jump = 1
    return pm, map, my_ch
    
def near(i, j):
    n_cor = []
    if 0 == i and 0 == j:
        n_cor.extend([(1,0), (0,1)])
    elif 3 == i and 0 == j:
        n_cor.extend([(2, 0), (3, 1)])
    elif 0 == i and 7 == j:
        n_cor.extend([(0, 6), (1, 7)])
    elif 3 == i and 7 == j:
        n_cor.extend([(3, 6), (2, 7)])
    elif 0 == j:
        n_cor.extend([(i-1, j), (i+1, j), (i, j+1)])
    elif 0 == i:
        n_cor.extend([(i, j-1), (i, j+1), (i+1, j)])
    elif 7 == j:
        n_cor.extend([(i-1, j), (i+1, j), (i, j-1)])
    elif 3 == i:
        n_cor.extend([(i, j-1), (i, j+1), (i-1, j)])
    else:
        n_cor.extend([(i-1, j), (i+1, j), (i, j+1), (i, j-1)])
    
    return n_cor
    
def mouse_position_to_block(mx, my, chess_back):
    global cstart_x
    global cstart_y
    global cstart_x2
    global cstart_y2
    
    for i in range(0, 4):
        for j in range(0, 4):
            if cstart_x+j*chess_back.get_width() < mx < cstart_x+(j+1)*chess_back.get_width() and cstart_y+i*chess_back.get_height() < my < cstart_y+(i+1)*chess_back.get_height():
                return (i, j)
        
    for i in range(0, 4):
        for j in range(0, 4):
            if cstart_x2+j*chess_back.get_width() < mx < cstart_x2+(j+1)*chess_back.get_width() and cstart_y2+i*chess_back.get_height() < my < cstart_y2+(i+1)*chess_back.get_height():
                return (i, 4+j)

def select_back_chess(map, my_ch):
    back_mark = [[0]*8, [0]*8, [0]*8, [0]*8]
    if 1 == check_back_exist(map, my_ch, back_mark):
        return random_select_back_chess(map, my_ch, back_mark)
    else:
        return (-1, -1)

def check_back_exist(map, my_ch, bm):
    back_exist = 0
    for i in range(0, 4):
        for j in range(0, 8):
            if map[i][j] != (-1, -1) and bm[i][j] != 1:
                if 1 == my_ch[map[i][j][0]][map[i][j][1]].back:
                    back_exist = 1
    return back_exist
    
def random_select_back_chess(map, my_ch, bm):    
    i = random.randint(0, 31)
    ii = 0
    
    while i != -1:
        y = ii/8
        x = ii%8
        if map[y][x] == (-1, -1) or bm[y][x] == 1:
            ii += 1
            if ii > 31:
                ii = ii%32
        elif 1 == my_ch[map[y][x][0]][map[y][x][1]].back:
            i -= 1
            if i < 0:
                break
            ii += 1
            if ii > 31:
                ii = ii%32
        else:
            ii += 1
            if ii > 31:
                ii = ii%32
    
    (y, x) = (ii/8, ii%8)
    return (y, x)
                
def chess_ai():
    global turn_id
    global first
    global my_ch
    global map
    global player_color
    global com_color
    global player_first
    global player_win
    
    pygame.display.update()
    
    if 0 == player_first and  1 == first:
        i = random.randint(0, 3) 
        j = random.randint(0, 7) 
        turn_id = my_ch[i][j].color
        my_ch[i][j].back = 0
        com_color = turn_id
        player_color = 1 - com_color
        first = 0
    elif turn_id == com_color and 0 == first:
        org, dest, score = deep_think(map, my_ch)
        print 'org', org, 'score', score
        if org == (-1, -1):
            if select_back_chess(map, my_ch) == (-1, -1):
                print 'player_win'
                player_win = 1
            else:
                print 'select_before'
                dest = select_back_chess(map, my_ch)
                print 'dest', dest
                my_ch[map[dest[0]][dest[1]][0]][map[dest[0]][dest[1]][1]].back = 0
        elif score < 30:
            print 'ls30', 
            dest = select_back_chess(map, my_ch)
            my_ch[map[dest[0]][dest[1]][0]][map[dest[0]][dest[1]][1]].back = 0
        else:
            print 'el'
            print 'org=', org, 'dest=', dest
            print 'com_color', com_color
            map, my_ch = move(org, dest, map, my_ch)
   
    if turn_id == com_color:
        turn_id = 1 - turn_id
        

def move_max_value(orgx, orgy, destx, desty, my_chess, map, org_value, opp_color, i, j):
    global max_value
    global mark
    
    if i == -1 or j == -1 or i == 4 or j == 8:
        return
    elif 1 == mark[i][j]:
        return
    
    mark[i][j] = 1
    
    if map[i][j] != (-1, -1): 
        if 1 == my_chess[map[i][j][0]][map[i][j][1]].back:
            return
        elif opp_color == my_chess[map[i][j][0]][map[i][j][1]].color:
            if 7 == org_value:
                if 1 == my_chess[map[i][j][0]][map[i][j][1]].value:
                    max_value = 8
                    return
                elif max_value < my_chess[map[i][j][0]][map[i][j][1]].value:
                    max_value = my_chess[map[i][j][0]][map[i][j][1]].value
                    return
            elif 1 == org_value:
                if 7 == my_chess[map[i][j][0]][map[i][j][1]].value:
                    max_value = 9
                    return
                elif max_value < my_chess[map[i][j][0]][map[i][j][1]].value:
                    max_value = my_chess[map[i][j][0]][map[i][j][1]].value
                    return
            elif max_value < my_chess[map[i][j][0]][map[i][j][1]].value:
                max_value = my_chess[map[i][j][0]][map[i][j][1]].value
                return
    elif orgy == desty and orgx+1 == destx:
        move_max_value(orgx, orgy, destx, desty, my_chess, map, org_value, opp_color, i, j+1)
        move_max_value(orgx, orgy, destx, desty, my_chess, map, org_value, opp_color, i+1, j)
        move_max_value(orgx, orgy, destx, desty, my_chess, map, org_value, opp_color, i-1, j)
    elif orgy == desty and orgx-1 == destx:
        move_max_value(orgx, orgy, destx, desty, my_chess, map, org_value, opp_color, i, j-1)
        move_max_value(orgx, orgy, destx, desty, my_chess, map, org_value, opp_color, i+1, j)
        move_max_value(orgx, orgy, destx, desty, my_chess, map, org_value, opp_color, i-1, j)
    elif orgy+1 == desty and orgx == destx:
        move_max_value(orgx, orgy, destx, desty, my_chess, map, org_value, opp_color, i+1, j)
        move_max_value(orgx, orgy, destx, desty, my_chess, map, org_value, opp_color, i, j+1)
        move_max_value(orgx, orgy, destx, desty, my_chess, map, org_value, opp_color, i, j-1)
    elif orgy-1 == desty and orgx == destx:
        move_max_value(orgx, orgy, destx, desty, my_chess, map, org_value, opp_color, i-1, j)
        move_max_value(orgx, orgy, destx, desty, my_chess, map, org_value, opp_color, i, j+1)
        move_max_value(orgx, orgy, destx, desty, my_chess, map, org_value, opp_color, i, j-1)    
        
def move_score(org, dest, my_chess, map):
    
    global king_live
    global max_value
    global mark
    
    (orgy, orgx) = org
    (desty, destx) = dest
    if map[desty][destx] == (-1, -1):
        max_value = 0
        mark = [[0]*8, [0]*8, [0]*8, [0]*8]
        opp_color = 1 - my_chess[map[orgy][orgx][0]][map[orgy][orgx][1]].color
        org_value = my_chess[map[orgy][orgx][0]][map[orgy][orgx][1]].value
        move_max_value(orgx, orgy, destx, desty, my_chess, map, org_value, opp_color, desty, destx)
        if max_value > 7:
            return 0
        elif max_value > org_value:
            return (-1)*max_value
        else:
            return max_value
    
    elif 1 == my_chess[map[desty][destx][0]][map[desty][destx][1]].live:
        print 'eat', eating_value_to_score(my_chess[map[desty][destx][0]][map[desty][destx][1]].value, king_live, my_chess[map[orgy][orgx][0]][map[orgy][orgx][1]].color)
        print 'org_color', my_chess[map[orgy][orgx][0]][map[orgy][orgx][1]].color
        return eating_value_to_score(my_chess[map[desty][destx][0]][map[desty][destx][1]].value, king_live, my_chess[map[orgy][orgx][0]][map[orgy][orgx][1]].color)

def move(org, dest, a_map, a_ch):
    global cor
    (orgi, orgj) = org
    (desti, destj) = dest
    
    print 'move', 'org', org, 'dest', dest
    
    #print 'b_a_map[desti][destj]', a_map[desti][destj]
    #print 'b_map[desti][destj]', map[desti][destj]
    
    if (-1, -1) == a_map[desti][destj]:
        org_ch = a_ch[a_map[orgi][orgj][0]][a_map[orgi][orgj][1]]
        (org_ch.row, org_ch.col) = (desti, destj)
        (org_ch.x, org_ch.y) = cor[org_ch.row][org_ch.col]
        a_map[desti][destj] = a_map[orgi][orgj]
        a_map[orgi][orgj] = (-1, -1)
    else:
        dest_ch = a_ch[a_map[desti][destj][0]][a_map[desti][destj][1]]
        org_ch  = a_ch[a_map[orgi][orgj][0]][a_map[orgi][orgj][1]]
        dest_ch.live = 0
        (org_ch.row, org_ch.col) = (desti, destj)
        (org_ch.x, org_ch.y) = cor[org_ch.row][org_ch.col]
        a_map[desti][destj] = a_map[orgi][orgj]
        a_map[orgi][orgj] = (-1, -1)
    
    #print 'af_a_map[desti][destj]', a_map[desti][destj]
    #print 'af_map[desti][destj]', map[desti][destj]
    
    return a_map, a_ch
        
def one_turn(a_map, a_ch, owner_color, sc, i = (-1, -1), j = (-1, -1)):
    global top_number
    global top_map
    global top_chess
    
    m = []
    
    a_map, a_ch = all_chess_move(a_map, a_ch)
    for chr in a_ch:
        for ch in chr:
            if ch.color == owner_color:
                for pm in ch.possible_move:
                    score = sc + move_score((ch.row, ch.col), pm, a_ch, a_map)
                    m.append([(ch.row, ch.col), pm, score])
    
    mf = []
    opp_color = 1 - owner_color
    
    for mm in m:
        af_map = copy.deepcopy(a_map)
        af_ch  = copy.deepcopy(a_ch)
        af_map, af_ch = move(mm[0], mm[1], af_map, af_ch)
        score = mm[2]
        af_map, af_ch = all_chess_move(af_map, af_ch)
        for chr in af_ch:
            for ch in chr:
                if ch.color == opp_color:
                    for pm in ch.possible_move:
                        score -= move_score((ch.row, ch.col), pm, af_ch, af_map)
                        mf.append([mm[0], mm[1], (ch.row, ch.col), pm, score])
                    
    if not mf and m:
        af2_map = copy.deepcopy(a_map)
        af2_ch  = copy.deepcopy(a_ch)
        sorted(m, key=lambda s: s[2])
        print 'm', m
        top_map = [0]
        top_chess = [0]
        top_map[0] = af2_map
        top_chess[0] = af2_ch
        print 's=', score
        if i == (-1, -1):
            return [[m[0][0], m[0][1], af2_map, af2_ch, m[0][2]]]
        else:
            return [[i, j, af2_map, af2_ch, m[0][2]]]
    
    sorted(mf, key=lambda ms: ms[4])   
    #print 'mf', mf
    
    num = top_number if top_number <= len(mf) else len(mf)
    
    top_map = [0] * num
    top_chess = [0] * num
    
    top_score = []    
    
    for ii in range(0, num):
        a2_map = copy.deepcopy(a_map)
        a2_ch = copy.deepcopy(a_ch)
        print 'ii move'
        top_map[ii], top_chess[ii] = move(mf[ii][0], mf[ii][1], a2_map, a2_ch)
        top_map[ii], top_chess[ii] = move(mf[ii][2], mf[ii][3], top_map[ii], top_chess[ii])
        if i == (-1, -1):
            top_score.append([mf[ii][0], mf[ii][1], top_map[ii], top_chess[ii], mf[ii][4]])
        else:
             top_score.append([i, j, top_map[ii], top_chess[ii], mf[ii][4]])
        
    return top_score
        
def deep_think(a_map, a_ch):
    global com_color
    global deep_level
    global top_number
    
    ts = copy.deepcopy(one_turn(a_map, a_ch, com_color, 0))
    t_map = top_map
    t_chess = top_chess
    if ts:
        score = ts[0][4]
    else:
        return (-1, -1), (-1, -1), -1
    
    tn = top_number if top_number <= len(ts) else len(ts)
    backup_top_score = ts
    
    for level in range(0, deep_level):
        max_top_score = []
        max_ts = [0] * tn
        for ii in range(0, tn):
            max_ts[ii] = copy.deepcopy(one_turn(t_map[ii], t_chess[ii], com_color, score, backup_top_score[ii][0], backup_top_score[ii][1]))
            max_top_score.extend(max_ts[ii])
        sorted(max_top_score, key=lambda s: s[4])
        tn = top_number if top_number <= len(max_top_score) else len(max_top_score)
        t_map = [0] * tn
        t_chess = [0] * tn
        backup_top_score = copy.deepcopy(max_top_score)
        for ii in range(0, tn):
            (t_map[ii], t_chess[ii]) = (max_top_score[ii][2], max_top_score[ii][3])
        if max_top_score:
            score = max_top_score[0][4]
        
    if max_top_score:
        return max_top_score[0][0], max_top_score[0][1], max_top_score[0][4]
    else:
        return (-1, -1), (-1, -1), -1
    
def eating_value_to_score(value, king, owner_color):
    opp_color = 1 - owner_color
    if 1 == value:
        if 1 == king[opp_color]:
            return 40
        else:
            return 5
    elif 2 == value:
        return 70
    elif 3 == value:
        return 30
    elif 4 == value:
        return 40
    elif 5 == value:
        return 70
    elif 6 == value:
        return 80
    elif 7 == value:
        return 90
        
def main():
    global cstart_x
    global cstart_y
    global cstart_x2
    global cstart_y2
    global chess_index
    global turn_id
    global player_color
    global com_color
    global player_first
    global my_ch
    global map
    global cor
    global first
    global player_win
    global chess_num
    
    while True:
        selected_c = None
        player_win = 0
        turn_id = 0
        player_color = 0
        com_color = 1
        first = 1
        moving = 1
        game_start = 1
        chess_num = [16, 16]
        
        player_first = random.randint(0, 1)
        
        chess_index = ini_random_chess(chess_index)
        for i in range(0, 4):
            for j in range(0, 4):
                ch = chess(chess_index[8*i+j],index_to_color(chess_index[8*i+j]), index_to_chess_value(chess_index[8*i+j]), (cstart_x+j*chess_back.get_width(),cstart_y+i*chess_back.get_height()), (i, j), chess_back.get_size(), index_to_chess_surface(chess_index[8*i+j]))
                my_ch[i][j] = ch
                cor[i][j] = (ch.x, ch.y)
                map[i][j] = (i, j)
        for i in range(0, 4):
            for j in range(0, 4):
                ch = chess(chess_index[8*i+4+j],index_to_color(chess_index[8*i+4+j]), index_to_chess_value(chess_index[8*i+4+j]), (cstart_x2+j*chess_back.get_width(),cstart_y2+i*chess_back.get_height()), (i, 4+j), chess_back.get_size(), index_to_chess_surface(chess_index[8*i+4+j]))
                my_ch[i][4+j] = ch
                cor[i][4+j] = (ch.x, ch.y)
                map[i][4+j] = (i, 4+j)
    
        while 0 == player_win:
            if game_start:
                sound_new.play()
                game_start = 0
            screen.blit(background, (0,0))
            
            if 1 == moving:
                map, my_ch = all_chess_move(map, my_ch)
                moving = 0
            
            if 0 == player_first and 1 == first:
                player_color = 1
            
            for event in pygame.event.get():
                if event.type == QUIT:
                    exit()
                elif event.type == pygame.MOUSEBUTTONDOWN and turn_id == player_color:
                    sound_click.play()
                    for chr in my_ch:
                        for chc in chr:
                            ch_index = chc.click(pygame.mouse.get_pos())
                            if ch_index:                       
                                if 1 == player_first and 1 == first:                            
                                    turn_id = index_to_color(ch_index)
                                    player_color = turn_id
                                    com_color = 1 - player_color
                                    first = 0
                                    turn_id = 1 - turn_id
                                elif -1 == ch_index and chc.color == player_color:
                                    selected_c = chc
                                elif ch_index != -1:
                                    turn_id = 1 - turn_id
                elif event.type == pygame.MOUSEBUTTONUP and turn_id == player_color:
                    if selected_c:
                        (mouseX, mouseY) = pygame.mouse.get_pos()
                        moving = 0
                        for pm in selected_c.possible_move:
                            if pm == mouse_position_to_block(mouseX, mouseY, chess_back):
                                if map[pm[0]][pm[1]] != (-1, -1):
                                    my_ch[map[pm[0]][pm[1]][0]][map[pm[0]][pm[1]][1]].live = 0
                                    chess_num[my_ch[map[pm[0]][pm[1]][0]][map[pm[0]][pm[1]][1]].color] -= 1
                                    sound_capture.play()
                                else:
                                    sound_move.play()
                                map[pm[0]][pm[1]] = map[selected_c.row][selected_c.col]
                                map[selected_c.row][selected_c.col] = (-1, -1)
                                selected_c.x = cor[pm[0]][pm[1]][0]
                                selected_c.y = cor[pm[0]][pm[1]][1]
                                selected_c.row = pm[0]
                                selected_c.col = pm[1]
                                moving = 1
                                turn_id = 1 - turn_id
                                break
                        
                        if 0 == moving:
                           (selected_c.x, selected_c.y) = cor[selected_c.row][selected_c.col] 
                    
                        selected_c.speed = 0
                        selected_c = None
                    else:
                        moving = 1
                        
            if selected_c:
                (mouseX, mouseY) = pygame.mouse.get_pos()
                dx = mouseX - selected_c.x
                dy = mouseY - selected_c.y
                dx -= selected_c.size[0]/2
                dy -= selected_c.size[1]/2
                selected_c.angle = 0.5*math.pi + math.atan2(dy, dx)
                selected_c.speed = math.hypot(dx, dy) * 0.1
            
            
            for cr in my_ch:
                for c in cr:
                    c.draw(screen, chess_back)
            
            chess_ai()
            
            if selected_c:
                selected_c.move()
                selected_c.draw(screen, chess_back)
            
            if 0 == chess_num[player_color]:
                player_win = -1
            elif 0 == chess_num[com_color]:
                player_win = 1
            
            if 1 == player_win:
                sound_win.play()
                pygame.display.update()
                time.sleep(5)
            elif -1 == player_win:
                sound_loss.play()
                pygame.display.update()
                time.sleep(5)
            
            pygame.display.update()
            
    exit()

if __name__ == "__main__":
    main()