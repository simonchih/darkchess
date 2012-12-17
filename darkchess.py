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

pygame.display.set_icon(pygame.image.load("Image/darkchess_default.png"))
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
text_x = 237
text_y = 16

player_win = 0
player_first = 0
first = 1
# turn_id, 0:black, 1:red, 2:process_moving
turn_id = 0
player_color = 0
com_color = 1
max_value = 0

chtemp = chess(0, 0, 0, (0, 0), (0, 0), chess_back.get_size(), chess_back)
my_ch = [[chtemp, chtemp, chtemp, chtemp, chtemp, chtemp, chtemp, chtemp], [chtemp, chtemp, chtemp, chtemp, chtemp, chtemp, chtemp, chtemp], [chtemp, chtemp, chtemp, chtemp, chtemp, chtemp, chtemp, chtemp], [chtemp, chtemp, chtemp, chtemp, chtemp, chtemp, chtemp, chtemp]]
chess_index = [0] * 32
map = [[(0,0)]*8, [(0,0)]*8, [(0,0)]*8, [(0,0)]*8]
cor = [[(0,0)]*8, [(0,0)]*8, [(0,0)]*8, [(0,0)]*8]
mark = [[0]*8, [0]*8, [0]*8, [0]*8]
king_live = [1, 1]
chess_num = [16, 16]
com_mv_map = [0, 0]
back_num = 32

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

def all_chess_move(a_map, my_chess):
    for rowm in a_map:
        for m in rowm:
            if m!= (-1, -1) and 0 == my_chess[m[0]][m[1]].back:
                my_chess[m[0]][m[1]].possible_move, a_map, my_chess= collect_possible_move(my_chess[m[0]][m[1]].row, my_chess[m[0]][m[1]].col, a_map, my_chess)
    return a_map, my_chess
    
def collect_possible_move(i, j, a_map, my_chess):
    
    pm = []
    ncor = near(i,j)
    for nc in ncor:
        if (-1, -1) == a_map[nc[0]][nc[1]]:
            pm.append(nc)
        elif my_chess[a_map[nc[0]][nc[1]][0]][a_map[nc[0]][nc[1]][1]].color != my_chess[a_map[i][j][0]][a_map[i][j][1]].color and 0 == my_chess[a_map[nc[0]][nc[1]][0]][a_map[nc[0]][nc[1]][1]].back:
            if 1 == my_chess[a_map[i][j][0]][a_map[i][j][1]].value and 7 == my_chess[a_map[nc[0]][nc[1]][0]][a_map[nc[0]][nc[1]][1]].value:
                pm.append(nc)
            elif 7 == my_chess[map[i][j][0]][a_map[i][j][1]].value and 1 == my_chess[a_map[nc[0]][nc[1]][0]][a_map[nc[0]][nc[1]][1]].value:
                pass
            elif my_chess[a_map[i][j][0]][a_map[i][j][1]].value != 2 and my_chess[a_map[i][j][0]][a_map[i][j][1]].value >= my_chess[a_map[nc[0]][nc[1]][0]][a_map[nc[0]][nc[1]][1]].value:
                pm.append(nc)
    if 2 == my_chess[a_map[i][j][0]][a_map[i][j][1]].value:
        jump = 0
        for ii in range(i-1, -1, -1):
            if 1 == jump and a_map[ii][j] != (-1, -1):
                if 1 == my_chess[a_map[ii][j][0]][a_map[ii][j][1]].back or my_chess[a_map[ii][j][0]][a_map[ii][j][1]].color == my_chess[a_map[i][j][0]][a_map[i][j][1]].color:
                    break
                else:
                    pm.append((ii, j))
                    break
            if a_map[ii][j] != (-1, -1):
                jump = 1
        jump = 0
        for ii in range(i+1, 4, 1):
            if 1 == jump and a_map[ii][j] != (-1, -1):
                if 1 == my_chess[a_map[ii][j][0]][a_map[ii][j][1]].back or my_chess[a_map[ii][j][0]][a_map[ii][j][1]].color == my_chess[a_map[i][j][0]][a_map[i][j][1]].color:
                    break
                else:
                    pm.append((ii, j))
                    break
            if a_map[ii][j] != (-1, -1):
                jump = 1
        jump = 0
        for jj in range(j-1, -1, -1):
            if 1 == jump and a_map[i][jj] != (-1, -1):
                if 1 == my_chess[a_map[i][jj][0]][a_map[i][jj][1]].back or my_chess[a_map[i][jj][0]][a_map[i][jj][1]].color == my_chess[a_map[i][j][0]][a_map[i][j][1]].color:
                    break
                else:
                    pm.append((i, jj))
                    break
            if a_map[i][jj] != (-1, -1):
                jump = 1
        jump = 0
        for jj in range(j+1, 8, 1):
            if 1 == jump and map[i][jj] != (-1, -1):
                if 1 == my_chess[a_map[i][jj][0]][a_map[i][jj][1]].back or my_chess[a_map[i][jj][0]][a_map[i][jj][1]].color == my_chess[a_map[i][j][0]][a_map[i][j][1]].color:
                    break
                else:
                    pm.append((i, jj))
                    break
            if a_map[i][jj] != (-1, -1):
                jump = 1
    return pm, a_map, my_chess
    
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

def select_back_chess(a_map, my_chess):
    back_mark = [[0]*8, [0]*8, [0]*8, [0]*8]
    if 1 == check_back_exist(a_map, my_chess, back_mark):
        return random_select_back_chess(a_map, my_chess, back_mark)
    else:
        return None

def check_back_exist(a_map, my_chess, bm):
    back_exist = 0
    for i in range(0, 4):
        for j in range(0, 8):
            if a_map[i][j] != (-1, -1) and bm[i][j] != 1:
                if 1 == my_chess[a_map[i][j][0]][a_map[i][j][1]].back:
                    back_exist = 1
    return back_exist
    
def random_select_back_chess(a_map, my_chess, bm):    
    i = random.randint(0, 31)
    ii = 0
    
    while i != -1:
        y = ii/8
        x = ii%8
        if a_map[y][x] == (-1, -1) or bm[y][x] == 1:
            ii += 1
            if ii > 31:
                ii = 0
        elif 1 == my_chess[a_map[y][x][0]][a_map[y][x][1]].back:
            i -= 1
            if i < 0:
                break
            ii += 1
            if ii > 31:
                ii = 0
        else:
            ii += 1
            if ii > 31:
                ii = 0
    
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
    global back_num
    
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
        org, dest, score = com_think(map, my_ch)
        print 'org', org, 'dest', dest, 'score', score
        if org == None:
            if select_back_chess(map, my_ch) == None:
                player_win = 1
            else:
                dest = select_back_chess(map, my_ch)
                sound_click.play()
                my_ch[map[dest[0]][dest[1]][0]][map[dest[0]][dest[1]][1]].back = 0
                back_num -= 1
        elif 1 == player_cant_move(my_ch): 
            if select_back_chess(map, my_ch) == None or score < -20:
                map, my_ch = move_s(org, dest, map, my_ch)
            else:
                dest = select_back_chess(map, my_ch)
                sound_click.play()
                my_ch[map[dest[0]][dest[1]][0]][map[dest[0]][dest[1]][1]].back = 0
                back_num -= 1
        elif score <= 0:
            map, my_ch = move_s(org, dest, map, my_ch)
        elif select_back_chess(map, my_ch):
            dest = select_back_chess(map, my_ch)
            sound_click.play()
            my_ch[map[dest[0]][dest[1]][0]][map[dest[0]][dest[1]][1]].back = 0
            back_num -= 1
        else:
            map, my_ch = move_s(org, dest, map, my_ch)
   
    if turn_id == com_color:
        turn_id = 2
        

def move_max_value(orgx, orgy, destx, desty, my_chess, a_map, org_value, opp_color, i, j):
    global max_value
    global mark
    
    if i == -1 or j == -1 or i == 4 or j == 8:
        return
    elif 1 == mark[i][j]:
        return
    
    mark[i][j] = 1
    
    if map[i][j] != (-1, -1): 
        if 1 == my_chess[a_map[i][j][0]][a_map[i][j][1]].back:
            return
        elif opp_color == my_chess[a_map[i][j][0]][a_map[i][j][1]].color:
            if 7 == org_value:
                if 1 == my_chess[a_map[i][j][0]][a_map[i][j][1]].value:
                    max_value = 8
                    return
                elif max_value < my_chess[a_map[i][j][0]][a_map[i][j][1]].value:
                    max_value = my_chess[a_map[i][j][0]][a_map[i][j][1]].value
                    return
            elif 1 == org_value:
                if 7 == my_chess[a_map[i][j][0]][a_map[i][j][1]].value:
                    max_value = 0
                    return
                elif max_value < my_chess[a_map[i][j][0]][a_map[i][j][1]].value:
                    max_value = my_chess[a_map[i][j][0]][a_map[i][j][1]].value
                    return
            elif max_value < my_chess[a_map[i][j][0]][a_map[i][j][1]].value:
                max_value = my_chess[a_map[i][j][0]][a_map[i][j][1]].value
                return
        else:
            return
    elif orgy == desty and orgx+1 == destx:
        move_max_value(orgx, orgy, destx, desty, my_chess, a_map, org_value, opp_color, i, j+1)
        move_max_value(orgx, orgy, destx, desty, my_chess, a_map, org_value, opp_color, i+1, j)
        move_max_value(orgx, orgy, destx, desty, my_chess, a_map, org_value, opp_color, i-1, j)
    elif orgy == desty and orgx-1 == destx:                
        move_max_value(orgx, orgy, destx, desty, my_chess, a_map, org_value, opp_color, i, j-1)
        move_max_value(orgx, orgy, destx, desty, my_chess, a_map, org_value, opp_color, i+1, j)
        move_max_value(orgx, orgy, destx, desty, my_chess, a_map, org_value, opp_color, i-1, j)
    elif orgy+1 == desty and orgx == destx:                
        move_max_value(orgx, orgy, destx, desty, my_chess, a_map, org_value, opp_color, i+1, j)
        move_max_value(orgx, orgy, destx, desty, my_chess, a_map, org_value, opp_color, i, j+1)
        move_max_value(orgx, orgy, destx, desty, my_chess, a_map, org_value, opp_color, i, j-1)
    elif orgy-1 == desty and orgx == destx:                
        move_max_value(orgx, orgy, destx, desty, my_chess, a_map, org_value, opp_color, i-1, j)
        move_max_value(orgx, orgy, destx, desty, my_chess, a_map, org_value, opp_color, i, j+1)
        move_max_value(orgx, orgy, destx, desty, my_chess, a_map, org_value, opp_color, i, j-1)    
        
def move_score(org, dest, my_chess, a_map, owner_color):
    
    global king_live
    global max_value
    global mark
    
    (orgy, orgx) = org
    (desty, destx) = dest
    if a_map[desty][destx] == (-1, -1):
        if owner_color == player_color:
            return 0
        if  2 == my_chess[a_map[orgy][orgx][0]][a_map[orgy][orgx][1]].value:
            return 0
        max_value = 0
        mark = [[0]*8, [0]*8, [0]*8, [0]*8]
        opp_color = 1 - my_chess[a_map[orgy][orgx][0]][a_map[orgy][orgx][1]].color
        org_value = my_chess[a_map[orgy][orgx][0]][a_map[orgy][orgx][1]].value
        move_max_value(orgx, orgy, destx, desty, my_chess, a_map, org_value, opp_color, desty, destx)
        if max_value > 7:
            return 0
        elif max_value == 0:
            return 7
        elif max_value > org_value:
            return (-1)*max_value
        else:
            return max_value
    
    elif 1 == my_chess[a_map[desty][destx][0]][a_map[desty][destx][1]].live:
        return eating_value_to_score(my_chess[a_map[desty][destx][0]][a_map[desty][destx][1]].value, king_live, my_chess[a_map[orgy][orgx][0]][a_map[orgy][orgx][1]].color)

def move_s(org, dest, a_map, a_ch):
    global cor
    global com_mv_map
    
    (orgi, orgj) = org
    (desti, destj) = dest
    
    #print 'b_a_map[desti][destj]', a_map[desti][destj]
    #print 'b_map[desti][destj]', map[desti][destj]
    
    if (-1, -1) == a_map[desti][destj]:
        org_ch = a_ch[a_map[orgi][orgj][0]][a_map[orgi][orgj][1]]
        (org_ch.row, org_ch.col) = (desti, destj)
        #(org_ch.x, org_ch.y) = cor[org_ch.row][org_ch.col]
        com_mv_map = list(a_map[orgi][orgj])
        a_map[orgi][orgj] = (-1, -1)
        sound_move.play()
    else:
        #dest_ch = a_ch[a_map[desti][destj][0]][a_map[desti][destj][1]]
        org_ch  = a_ch[a_map[orgi][orgj][0]][a_map[orgi][orgj][1]]
        #dest_ch.live = 0
        (org_ch.row, org_ch.col) = (desti, destj)
        #(org_ch.x, org_ch.y) = cor[org_ch.row][org_ch.col]
        com_mv_map = list(a_map[orgi][orgj])
        a_map[orgi][orgj] = (-1, -1)
        sound_capture.play()
    
    #print 'af_a_map[desti][destj]', a_map[desti][destj]
    #print 'af_map[desti][destj]', map[desti][destj]
    
    return a_map, a_ch    
        
def move(org, dest, a_map, a_ch):
    global cor
    
    (orgi, orgj) = org
    (desti, destj) = dest
    
    if (-1, -1) == a_map[desti][destj]:
        org_ch = a_ch[a_map[orgi][orgj][0]][a_map[orgi][orgj][1]]
        (org_ch.row, org_ch.col) = (desti, destj)
        (org_ch.x, org_ch.y) = cor[org_ch.row][org_ch.col]
        a_map[desti][destj] = (list(a_map[orgi][orgj])[0], list(a_map[orgi][orgj])[1])
        a_map[orgi][orgj] = (-1, -1)
    else:
        dest_ch = a_ch[a_map[desti][destj][0]][a_map[desti][destj][1]]
        org_ch  = a_ch[a_map[orgi][orgj][0]][a_map[orgi][orgj][1]]
        dest_ch.live = 0
        (org_ch.row, org_ch.col) = (desti, destj)
        (org_ch.x, org_ch.y) = cor[org_ch.row][org_ch.col]
        a_map[desti][destj] = (list(a_map[orgi][orgj])[0], list(a_map[orgi][orgj])[1])
        a_map[orgi][orgj] = (-1, -1)
    
    return a_map, a_ch

def player_cant_move(a_ch):
    for chr in a_ch:
        for ch in chr:
            if ch.color == player_color and 1 == ch.live:
                for pm in ch.possible_move:
                    return 0
    return 1
    
def com_think(a_map, a_ch):

    m = []
    
    max_score = 1
    sc = 0
    
    a_map, a_ch = all_chess_move(a_map, a_ch)
    for chr in a_ch:
        for ch in chr:
            if ch.color == com_color and 1 == ch.live:
                for pm in ch.possible_move:
                    score = sc - move_score((ch.row, ch.col), pm, a_ch, a_map, com_color)
                    m.append(((ch.row, ch.col), pm, score))
                    if score < max_score:
                        max_score = score
                        org = (ch.row, ch.col)
                        dest = pm   
    if m:
        mf = []
        brk = 0
        for mm in m:
            score2, m2, a2_map, a2_ch= one_turn(a_map, a_ch, mm, player_color, mm[0], mm[1], mm[2], 0.9)
            if score2:
                mf.append((mm[0], mm[1], score2))
                brk = 1
            elif m2:
                m2 = sorted(m2, key=lambda s:s[4])
                mf.append((mm[0], mm[1], m2[-1][4]))
            else:
                brk = 1
            if 0 == brk:
                score2, m3, a3_map, a3_ch= one_turn(a2_map, a2_ch, mm, com_color, m2[-1][2], m2[-1][3], m2[-1][4], 0.5)
                if score2:
                    mf.append((mm[0], mm[1], score2))
                    brk = 1
                elif m3:
                    m3 = sorted(m3, key=lambda s:s[4])
                else:
                    brk = 1
            else:
                mf.append((mm[0], mm[1], mm[2]))
                brk = 2
            if 0 == brk:
                score2, m4, a4_map, a4_ch= one_turn(a3_map, a3_ch, mm, player_color, m3[0][0], m3[0][1], m3[0][4], 0.5)
                if score2:
                    mf.append((mm[0], mm[1], score2))
                elif m4:
                    m4 = sorted(m4, key=lambda s:s[4])
                    mf.append((mm[0], mm[1], m4[-1][4]))
                else:
                    mf.append((mm[0], mm[1], m3[0][4]))
            elif 1 == brk:
                mf.append((mm[0], mm[1], m2[-1][4]))
        if mf:
            mf = sorted(mf, key=lambda s:s[2])
            print 'mf', mf
            return mf[0][0], mf[0][1], mf[0][2]
        else:
            return org, dest, max_score
    else:
        return None, None, None     

def one_turn(a_map, a_ch, mm, owner_color, nexti, nextj, sc, div):
    m2 = []
    af_map = copy.deepcopy(a_map)
    af_ch = copy.deepcopy(a_ch)
    af_map, af_ch = move(nexti, nextj, af_map, af_ch)
    af_map, af_ch = all_chess_move(af_map, af_ch)
    print 'mm', mm
    if owner_color == player_color and 1 == player_cant_move(af_ch):
        print 'mm out', 'mm[0], [1], [2]', mm[0], mm[1], mm[2]
        return mm[2], None, None, None
        
    for chr in af_ch:
        for ch in chr:
            if ch.color == owner_color and 1 == ch.live:
                for pm in ch.possible_move:
                    if owner_color == player_color:
                        score = sc + div * move_score((ch.row, ch.col), pm, af_ch, af_map, player_color)
                    else:
                        score = sc - div * move_score((ch.row, ch.col), pm, af_ch, af_map, com_color)
                    m2.append((mm[0], mm[1], (ch.row, ch.col), pm, score))
                    
    return None, m2, af_map, af_ch

        
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

def display_font():
    
    if 1 == player_win:
        winer = u"玩家勝!!!"
        screen.blit(write(winer, (0, 0, 255)), (text_x, text_y))
    elif -1 == player_win:
        winer = u"電腦勝..."
        screen.blit(write(winer, (0, 255, 0)), (text_x, text_y))
    elif 1 == first:
        if 1 == player_first:
            up = u"玩家先"
            screen.blit(write(up, (150, 150, 150)), (text_x, text_y))
        else:
            uc = u"電腦先"
            screen.blit(write(uc, (150, 150, 150)), (text_x, text_y))
    elif turn_id == player_color:
        up = u"玩家下"
        if 1 == player_color:
            screen.blit(write(up, (255, 0, 0)), (text_x, text_y))
        else:
            screen.blit(write(up, (0, 0, 0)), (text_x, text_y))
    else: #turn_id == com_color or turn_id == 2
        uc = u"電腦下"
        if 1 == com_color:
            screen.blit(write(uc, (255, 0, 0)), (text_x, text_y))
        else:
            screen.blit(write(uc, (0, 0, 0)), (text_x, text_y))       
        
def write(msg="pygame is cool", color= (0,0,0)):    
    #myfont = pygame.font.SysFont("None", 32) #To avoid py2exe error
    myfont = pygame.font.Font("wqy-zenhei.ttf",14)
    mytext = myfont.render(msg, True, color)
    mytext = mytext.convert_alpha()
    return mytext 
        
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
    global com_moving_end
    global back_num
    
    while True:
        selected_c = None
        player_win = 0
        turn_id = 0
        player_color = 0
        com_color = 1
        first = 1
        moving = 1
        com_mv = 0
        com_moving_end = 1
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
            
            for event in pygame.event.get():
                if event.type == QUIT:
                    exit()
                elif event.type == pygame.MOUSEBUTTONDOWN and turn_id == player_color:
                    map, my_ch = all_chess_move(map, my_ch)
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
                                    selected_c = None
                                    print 'ch_index first', ch_index
                                    turn_id = com_color
                                elif -1 == ch_index and chc.color == player_color:
                                    selected_c = chc
                                elif ch_index != -1:
                                    selected_c = None
                                    back_num -= 1
                                    print 'ch_index', ch_index
                                    turn_id = com_color
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
                                if map[selected_c.row][selected_c.col] == (-1, -1):
                                    print 'map selected_c error', 'row', selected_c.row, 'col', selected_c.col
                                    print crash
                                map[pm[0]][pm[1]] = map[selected_c.row][selected_c.col]
                                map[selected_c.row][selected_c.col] = (-1, -1)
                                selected_c.x = cor[pm[0]][pm[1]][0]
                                selected_c.y = cor[pm[0]][pm[1]][1]
                                selected_c.row = pm[0]
                                selected_c.col = pm[1]
                                moving = 1
                                print 'move down', 'row', selected_c.row, 'col', selected_c.col
                                turn_id = com_color
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
            
            display_font()
            for cr in my_ch:
                for c in cr:
                    c.draw(screen, chess_back)
            
            com_mv = 0
            no_move = 1
            if 2 == turn_id:
                for cr in my_ch:
                    for c in cr:
                        if c.x != cor[c.row][c.col][0]:
                            c.x = c.x+1 if c.x < cor[c.row][c.col][0] else c.x-1
                            com_mv = 1
                            no_move = 0
                            if (c.x, c.y) == cor[c.row][c.col]:
                                (desti, destj) = map[c.row][c.col]
                                if (desti, destj) != (-1, -1):
                                    dest_ch = my_ch[desti][destj]
                                    dest_ch.live = 0
                                    chess_num[dest_ch.color] -= 1
                                map[c.row][c.col] = (com_mv_map[0], com_mv_map[1])
                                turn_id = player_color
                        if c.y != cor[c.row][c.col][1]:
                            c.y = c.y+1 if c.y < cor[c.row][c.col][1] else c.y-1
                            com_mv = 1
                            no_move = 0
                            if (c.x, c.y) == cor[c.row][c.col]:
                                (desti, destj) = map[c.row][c.col]
                                if (desti, destj) != (-1, -1):
                                    dest_ch = my_ch[desti][destj]
                                    dest_ch.live = 0
                                    chess_num[dest_ch.color] -= 1
                                map[c.row][c.col] = (com_mv_map[0], com_mv_map[1])
                                turn_id = player_color
                        if 1 == com_mv:
                            c.draw(screen, chess_back)
                            com_mv = 0
                if 1 == no_move:
                    turn_id = player_color
            
            chess_ai()
            
            if selected_c:
                selected_c.move()
                selected_c.draw(screen, chess_back)
            
            if 0 == chess_num[player_color]:
                player_win = -1
                
            elif 0 == chess_num[com_color]:
                player_win = 1
            
            if 1 == player_win:
                screen.blit(background, (0,0))
                display_font()
                for cr in my_ch:
                    for c in cr:
                        c.draw(screen, chess_back)
                sound_win.play()
                pygame.display.update()
                time.sleep(5)
            elif -1 == player_win:
                screen.blit(background, (0,0))
                display_font()
                for cr in my_ch:
                    for c in cr:
                        c.draw(screen, chess_back)
                sound_loss.play()
                pygame.display.update()
                time.sleep(5)
            
            pygame.display.update()
            
    exit()

if __name__ == "__main__":
    main()