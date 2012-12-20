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
open_score = None

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
    for chr in my_chess:
        for ch in chr:
            if 0 == ch.back and 1 == ch.live:
                ch.possible_move, a_map, my_chess= collect_possible_move(ch.row, ch.col, a_map, my_chess)
    return a_map, my_chess
    
def collect_possible_move(i, j, a_map, my_chess):
    
    pm = []
    ncor = near(i,j)
    for nc in ncor:
        if None == a_map[nc[0]][nc[1]]:
            pm.append(nc)
        elif a_map[i][j] != None: #a_map[i][j] != None and a_map[nc[0]][nc[1]] != None 
            if my_chess[a_map[nc[0]][nc[1]][0]][a_map[nc[0]][nc[1]][1]].color != my_chess[a_map[i][j][0]][a_map[i][j][1]].color and 0 == my_chess[a_map[nc[0]][nc[1]][0]][a_map[nc[0]][nc[1]][1]].back:
                if 1 == my_chess[a_map[i][j][0]][a_map[i][j][1]].value and 7 == my_chess[a_map[nc[0]][nc[1]][0]][a_map[nc[0]][nc[1]][1]].value:
                    pm.append(nc)
                elif 7 == my_chess[a_map[i][j][0]][a_map[i][j][1]].value and 1 == my_chess[a_map[nc[0]][nc[1]][0]][a_map[nc[0]][nc[1]][1]].value:
                    pass
                elif my_chess[a_map[i][j][0]][a_map[i][j][1]].value != 2 and my_chess[a_map[i][j][0]][a_map[i][j][1]].value >= my_chess[a_map[nc[0]][nc[1]][0]][a_map[nc[0]][nc[1]][1]].value:
                    pm.append(nc)
    if a_map[i][j] != None:
        if 2 == my_chess[a_map[i][j][0]][a_map[i][j][1]].value:
            jump = 0
            for ii in range(i-1, -1, -1):
                if 1 == jump and a_map[ii][j] != None:
                    if 1 == my_chess[a_map[ii][j][0]][a_map[ii][j][1]].back or my_chess[a_map[ii][j][0]][a_map[ii][j][1]].color == my_chess[a_map[i][j][0]][a_map[i][j][1]].color:
                        break
                    else:
                        pm.append((ii, j))
                        break
                if a_map[ii][j] != None:
                    jump = 1
            jump = 0
            for ii in range(i+1, 4, 1):
                if 1 == jump and a_map[ii][j] != None:
                    if 1 == my_chess[a_map[ii][j][0]][a_map[ii][j][1]].back or my_chess[a_map[ii][j][0]][a_map[ii][j][1]].color == my_chess[a_map[i][j][0]][a_map[i][j][1]].color:
                        break
                    else:
                        pm.append((ii, j))
                        break
                if a_map[ii][j] != None:
                    jump = 1
            jump = 0
            for jj in range(j-1, -1, -1):
                if 1 == jump and a_map[i][jj] != None:
                    if 1 == my_chess[a_map[i][jj][0]][a_map[i][jj][1]].back or my_chess[a_map[i][jj][0]][a_map[i][jj][1]].color == my_chess[a_map[i][j][0]][a_map[i][j][1]].color:
                        break
                    else:
                        pm.append((i, jj))
                        break
                if a_map[i][jj] != None:
                    jump = 1
            jump = 0
            for jj in range(j+1, 8, 1):
                if 1 == jump and a_map[i][jj] != None:
                    if 1 == my_chess[a_map[i][jj][0]][a_map[i][jj][1]].back or my_chess[a_map[i][jj][0]][a_map[i][jj][1]].color == my_chess[a_map[i][j][0]][a_map[i][j][1]].color:
                        break
                    else:
                        pm.append((i, jj))
                        break
                if a_map[i][jj] != None:
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
            if a_map[i][j] != None and bm[i][j] != 1:
                if 1 == my_chess[a_map[i][j][0]][a_map[i][j][1]].back:
                    back_exist = 1
    return back_exist
    
def random_select_back_chess(a_map, my_chess, bm):    
    i = random.randint(0, 31)
    ii = 0
    
    while i != -1:
        y = ii/8
        x = ii%8
        if a_map[y][x] == None or bm[y][x] == 1:
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
        back_num -= 1
        com_color = turn_id
        player_color = 1 - com_color
        first = 0
    elif turn_id == com_color and 0 == first:
        print 'back_num', back_num
        org, dest, score = com_think(map, my_ch)
        print 'org', org, 'dest', dest, 'score', score, 'op score', open_score
        if 0 == back_num and 1 == cant_move(map, my_ch, com_color):
            player_win = 1
        if back_num > 0:
            if org == None or score > open_score - 4:
                dest = select_back_chess(map, my_ch)
                sound_click.play()
                my_ch[map[dest[0]][dest[1]][0]][map[dest[0]][dest[1]][1]].back = 0
                back_num -= 1 
            elif score == open_score:
                if score >= 0:
                    dest = select_back_chess(map, my_ch)
                    sound_click.play()
                    my_ch[map[dest[0]][dest[1]][0]][map[dest[0]][dest[1]][1]].back = 0
                    back_num -= 1
                else:
                    map, my_ch = move_s(org, dest, map, my_ch)
            else:
                map, my_ch = move_s(org, dest, map, my_ch)
        elif 0 == player_win:
            map, my_ch = move_s(org, dest, map, my_ch) 
   
    if turn_id == com_color:
        turn_id = 2
        

def move_max_value(orgx, orgy, destx, desty, my_chess, a_map, org_value, owner_color, i, j):
    global max_value
    global mark
    
    if i == -1 or j == -1 or i == 4 or j == 8:
        return
    elif 1 == mark[i][j]:
        return
    elif a_map[i][j] != None:
        if 1 == my_chess[a_map[i][j][0]][a_map[i][j][1]].back:
            return
        elif owner_color == my_chess[a_map[i][j][0]][a_map[i][j][1]].color:
            return
    
    mark[i][j] = 1
    
    if map[i][j] != None:
        if 7 == org_value:
            if 1 == my_chess[a_map[i][j][0]][a_map[i][j][1]].value:
                max_value = 8
                return
            elif max_value < my_chess[a_map[i][j][0]][a_map[i][j][1]].value:
                max_value = my_chess[a_map[i][j][0]][a_map[i][j][1]].value
                return
        elif 1 == org_value:
            if 7 == my_chess[a_map[i][j][0]][a_map[i][j][1]].value:
                max_value = 9
                return
            elif max_value < my_chess[a_map[i][j][0]][a_map[i][j][1]].value:
                max_value = my_chess[a_map[i][j][0]][a_map[i][j][1]].value
                return
        elif max_value < my_chess[a_map[i][j][0]][a_map[i][j][1]].value:
            max_value = my_chess[a_map[i][j][0]][a_map[i][j][1]].value
            return
    elif orgy == desty and orgx+1 == destx:
        move_max_value(orgx, orgy, destx, desty, my_chess, a_map, org_value, owner_color, i, j+1)
        move_max_value(orgx, orgy, destx, desty, my_chess, a_map, org_value, owner_color, i+1, j)
        move_max_value(orgx, orgy, destx, desty, my_chess, a_map, org_value, owner_color, i-1, j)
    elif orgy == desty and orgx-1 == destx:                
        move_max_value(orgx, orgy, destx, desty, my_chess, a_map, org_value, owner_color, i, j-1)
        move_max_value(orgx, orgy, destx, desty, my_chess, a_map, org_value, owner_color, i+1, j)
        move_max_value(orgx, orgy, destx, desty, my_chess, a_map, org_value, owner_color, i-1, j)
    elif orgy+1 == desty and orgx == destx:                
        move_max_value(orgx, orgy, destx, desty, my_chess, a_map, org_value, owner_color, i+1, j)
        move_max_value(orgx, orgy, destx, desty, my_chess, a_map, org_value, owner_color, i, j+1)
        move_max_value(orgx, orgy, destx, desty, my_chess, a_map, org_value, owner_color, i, j-1)
    elif orgy-1 == desty and orgx == destx:                
        move_max_value(orgx, orgy, destx, desty, my_chess, a_map, org_value, owner_color, i-1, j)
        move_max_value(orgx, orgy, destx, desty, my_chess, a_map, org_value, owner_color, i, j+1)
        move_max_value(orgx, orgy, destx, desty, my_chess, a_map, org_value, owner_color, i, j-1)    

def near2_have_same_value(org, my_chess, a_map, owner_color):
    if org == None:
        return 0
    elif owner_color == player_color:
        return 0
    
    (orgy, orgx) = org
    
    m = a_map[orgy][orgx]
    if m == None:
        return 0
    
    if orgy-2 >= 0:
        n = a_map[orgy-2][orgx]
        if n == None:
            return 0
        elif my_chess[n[0]][n[1]].color != my_chess[m[0]][m[1]].color and my_chess[n[0]][n[1]].value == my_chess[m[0]][m[1]].value:
            return 1
    if orgy+2 <= 3:
        n = a_map[orgy+2][orgx]
        if n == None:
            return 0
        elif my_chess[n[0]][n[1]].color != my_chess[m[0]][m[1]].color and my_chess[n[0]][n[1]].value == my_chess[m[0]][m[1]].value:
            return 1
    if orgx-2 >= 0:
        n = a_map[orgy][orgx-2]
        if n == None:
            return 0
        elif my_chess[n[0]][n[1]].color != my_chess[m[0]][m[1]].color and my_chess[n[0]][n[1]].value == my_chess[m[0]][m[1]].value:
            return 1
    if orgx+2 <= 7:
        n = a_map[orgy][orgx+2]
        if n == None:
            return 0
        elif my_chess[n[0]][n[1]].color != my_chess[m[0]][m[1]].color and my_chess[n[0]][n[1]].value == my_chess[m[0]][m[1]].value:
            return 1
    if orgy-1 >= 0 and orgx-1 >=0:
        n = a_map[orgy-1][orgx-1]
        if n == None:
            return 0
        elif my_chess[n[0]][n[1]].color != my_chess[m[0]][m[1]].color and my_chess[n[0]][n[1]].value == my_chess[m[0]][m[1]].value:
            return 1
    if orgy-1 >= 0 and orgx+1 <=7:
        n = a_map[orgy-1][orgx+1]
        if n == None:
            return 0
        elif my_chess[n[0]][n[1]].color != my_chess[m[0]][m[1]].color and my_chess[n[0]][n[1]].value == my_chess[m[0]][m[1]].value:
            return 1
    if orgy+1 <= 3 and orgx-1 >= 0:
        n = a_map[orgy+1][orgx-1]
        if n == None:
            return 0
        elif my_chess[n[0]][n[1]].color != my_chess[m[0]][m[1]].color and my_chess[n[0]][n[1]].value == my_chess[m[0]][m[1]].value:
            return 1
    if orgy+1 <= 3 and orgx+1 <= 7:
        n = a_map[orgy+1][orgx+1]
        if n == None:
            return 0
        elif my_chess[n[0]][n[1]].color != my_chess[m[0]][m[1]].color and my_chess[n[0]][n[1]].value == my_chess[m[0]][m[1]].value:
            return 1
        
def move_score(org, dest, my_chess, a_map, owner_color):
    
    global king_live
    global max_value
    global mark
    
    (orgy, orgx) = org
    (desty, destx) = dest
    if a_map[desty][destx] == None:
        if owner_color == player_color:
            return 0
        if  2 == my_chess[a_map[orgy][orgx][0]][a_map[orgy][orgx][1]].value:
            return 0
        
        max_value = 0
        mark = [[0]*8, [0]*8, [0]*8, [0]*8]
        org_value = my_chess[a_map[orgy][orgx][0]][a_map[orgy][orgx][1]].value
        if 1 == near2_have_same_value(org, my_chess, a_map, owner_color):
            return -1
        move_max_value(orgx, orgy, destx, desty, my_chess, a_map, org_value, my_chess[a_map[orgy][orgx][0]][a_map[orgy][orgx][1]].color, desty, destx)
        if 8 == max_value:
            return org_value/2
        elif 9 == max_value:
            return 7
        elif max_value > org_value:
            return org_value/2
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
    
    if org == dest:
        print crash
    if None == a_map[desti][destj]:
        org_ch = a_ch[a_map[orgi][orgj][0]][a_map[orgi][orgj][1]]
        (org_ch.row, org_ch.col) = (desti, destj)
        #(org_ch.x, org_ch.y) = cor[org_ch.row][org_ch.col]
        com_mv_map = list(a_map[orgi][orgj])
        a_map[orgi][orgj] = None
        sound_move.play()
    else:
        #dest_ch = a_ch[a_map[desti][destj][0]][a_map[desti][destj][1]]
        org_ch  = a_ch[a_map[orgi][orgj][0]][a_map[orgi][orgj][1]]
        #dest_ch.live = 0
        (org_ch.row, org_ch.col) = (desti, destj)
        #(org_ch.x, org_ch.y) = cor[org_ch.row][org_ch.col]
        com_mv_map = list(a_map[orgi][orgj])
        a_map[orgi][orgj] = None
        sound_capture.play()
    
    #print 'af_a_map[desti][destj]', a_map[desti][destj]
    #print 'af_map[desti][destj]', map[desti][destj]
    
    return a_map, a_ch    
        
def move(org, dest, a_map, a_ch):
    global cor
    
    if org == dest:
        return a_map, a_ch
    
    (orgi, orgj) = org
    (desti, destj) = dest
    
    if None == a_map[desti][destj]:
        org_ch = a_ch[a_map[orgi][orgj][0]][a_map[orgi][orgj][1]]
        (org_ch.row, org_ch.col) = (desti, destj)
        (org_ch.x, org_ch.y) = cor[org_ch.row][org_ch.col]
        a_map[desti][destj] = (list(a_map[orgi][orgj])[0], list(a_map[orgi][orgj])[1])
        a_map[orgi][orgj] = None
    else:
        dest_ch = a_ch[a_map[desti][destj][0]][a_map[desti][destj][1]]
        org_ch  = a_ch[a_map[orgi][orgj][0]][a_map[orgi][orgj][1]]
        dest_ch.live = 0
        (org_ch.row, org_ch.col) = (desti, destj)
        (org_ch.x, org_ch.y) = cor[org_ch.row][org_ch.col]
        a_map[desti][destj] = (list(a_map[orgi][orgj])[0], list(a_map[orgi][orgj])[1])
        a_map[orgi][orgj] = None
    
    return a_map, a_ch    
    
def cant_move(a_map, a_ch, owner_color):
    a_map, a_ch = all_chess_move(a_map, a_ch)
    for chr in a_ch:
        for ch in chr:
            if ch.color == owner_color and 1 == ch.live:
                for pm in ch.possible_move:
                    return 0
    return 1

def temp_clac_num_back():
    cb = 0
    for chr in my_ch:
        for ch in chr:
            if ch.back == 1:
                cb += 1
    return cb
    
def com_think(a_map, a_ch):
    global open_score

    m = []
    
    max_score = 1
    sc = 0
    
    a_map, a_ch = all_chess_move(a_map, a_ch)
    
    if back_num > 0:
        open_score = 0
        m.append((None, None, 0))
        max_score = 0
        org = None
        dest = None
    else:
        open_score = None
    
    for chr in a_ch:
        for ch in chr:
            if ch.color == com_color and 1 == ch.live:
                for pm in ch.possible_move:
                    score = sc - move_score((ch.row, ch.col), pm, a_ch, a_map, com_color)
                    m.append(((ch.row, ch.col), pm, score))
                    #print 'm', m
                    if score < max_score:
                        max_score = score
                        org = (ch.row, ch.col)
                        dest = pm   
    if len(m) > 1:
        mf = []
        m2 = []
        m3 = []
        m4 = []
        for mm in m:
            m2, a2_map, a2_ch= one_turn(a_map, a_ch, mm, player_color, mm[0], mm[1], mm[2], 0.999)
            if m2:
                m2 = sorted(m2, key=lambda s:s[4])
                if mm[0] == mm[1]:
                    open_score = m2[-1][4]
                if m2[-1][4] == mm[2]:
                    a2_map = a_map
                    a2_ch = a_ch
                mf.append((mm[0], mm[1], m2[-1][4]))
                if mm[0] == mm[1]:
                    continue
                    m3, a3_map, a3_ch= one_turn(a2_map, a2_ch, mm, com_color, m2[-1][2], m2[-1][3], m2[-1][4], 0.998)
                if m3:
                    m3 = sorted(m3, key=lambda s:s[4])
                    m4, a4_map, a4_ch= one_turn(a3_map, a3_ch, mm, player_color, m3[0][0], m3[0][1], m3[0][4], 0.997)
                    if m4:
                        m4 = sorted(m4, key=lambda s:s[4])
                        mf.append((mm[0], mm[1], m4[-1][4]))
        if mf:
            mf = sorted(mf, key=lambda s:s[2])
            print 'mf', mf
            return mf[0][0], mf[0][1], mf[0][2]
        else:
            return org, dest, max_score
    elif 1 == len(m):
        return m[0][0], m[0][1], m[0][2]
    else:
        return None, None, 0     

def one_turn(a_map, a_ch, mm, owner_color, nexti, nextj, sc, div):
    m2 = []
    af_map = copy.deepcopy(a_map)
    af_ch = copy.deepcopy(a_ch)
    af_map, af_ch = move(nexti, nextj, af_map, af_ch)
    af_map, af_ch = all_chess_move(af_map, af_ch)
    
    if owner_color == player_color and 1 == cant_move(af_map, af_ch, player_color):
        m2.append((mm[0], mm[1], None, None, mm[2]))
        return m2, af_map, af_ch

    if back_num > 0:
        m2.append((mm[0], mm[1], None, None, mm[2]))
    for chr in af_ch:
        for ch in chr:
            if ch.color == owner_color and 1 == ch.live:
                #print 'ch.row', ch.row, 'ch.col', ch.col, 'pm', ch.possible_move 
                for pm in ch.possible_move:
                    if owner_color == player_color:
                        score = sc + div * move_score((ch.row, ch.col), pm, af_ch, af_map, player_color)
                    else:
                        score = sc - div * move_score((ch.row, ch.col), pm, af_ch, af_map, com_color)
                    m2.append((mm[0], mm[1], (ch.row, ch.col), pm, score))
                    
    return m2, af_map, af_ch

        
def eating_value_to_score(value, king, owner_color):
    opp_color = 1 - owner_color
    if 1 == value:
        if 1 == king[opp_color]:
            return 19
        else:
            return 7
    elif 2 == value:
        return 79
    elif 3 == value:
        return 15
    elif 4 == value:
        return 39
    elif 5 == value:
        return 79
    elif 6 == value:
        return 159
    elif 7 == value:
        return 319

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
        back_num = 32
        
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
            if 1 == game_start:
                sound_new.play()
                game_start = 0

            screen.blit(background, (0,0))
            
            if turn_id == player_color:
                if 0 == back_num and 1 == cant_move(map, my_ch, player_color):
                    player_win = -1
                
                for event in pygame.event.get():
                    if event.type == QUIT:
                        exit()
                    elif event.type == pygame.MOUSEBUTTONDOWN and turn_id == player_color:
                        map, my_ch = all_chess_move(map, my_ch)
                        sound_click.play()
                        click_once = 0
                        for chr in my_ch:
                            for chc in chr:
                                if 0 == click_once:
                                    ch_index = chc.click(pygame.mouse.get_pos())
                                    if ch_index != None:
                                        if 1 == player_first and 1 == first:                            
                                            turn_id = index_to_color(ch_index)
                                            player_color = turn_id
                                            com_color = 1 - player_color
                                            first = 0
                                            selected_c = None
                                            back_num -= 1
                                            turn_id = com_color
                                        elif -1 == ch_index and chc.color == player_color:
                                            selected_c = chc
                                        elif ch_index != -1:
                                            selected_c = None
                                            back_num -= 1
                                            turn_id = com_color
                                        click_once = 1
                                        break
                    elif event.type == pygame.MOUSEBUTTONUP and turn_id == player_color:
                        if selected_c != None:
                            (mouseX, mouseY) = pygame.mouse.get_pos()
                            moving = 0
                            for pm in selected_c.possible_move:
                                if pm == mouse_position_to_block(mouseX, mouseY, chess_back):
                                    if map[pm[0]][pm[1]] != None:
                                        my_ch[map[pm[0]][pm[1]][0]][map[pm[0]][pm[1]][1]].live = 0
                                        chess_num[my_ch[map[pm[0]][pm[1]][0]][map[pm[0]][pm[1]][1]].color] -= 1
                                        sound_capture.play()
                                    else:
                                        sound_move.play()
                                    map[pm[0]][pm[1]] = map[selected_c.row][selected_c.col]
                                    map[selected_c.row][selected_c.col] = None
                                    selected_c.x = cor[pm[0]][pm[1]][0]
                                    selected_c.y = cor[pm[0]][pm[1]][1]
                                    selected_c.row = pm[0]
                                    selected_c.col = pm[1]
                                    moving = 1
                                    turn_id = com_color
                                    break
                            
                            if 0 == moving:
                               (selected_c.x, selected_c.y) = cor[selected_c.row][selected_c.col] 
                        
                            selected_c.speed = 0
                            selected_c = None
                            moving = 1
                            break
                        else:
                            moving = 1
            
            if 1 == moving:
                map, my_ch = all_chess_move(map, my_ch)
                moving = 0
            
            if selected_c != None:
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
                       
            no_move = 1
            if 2 == turn_id:
                for cr in my_ch:
                    for c in cr:
                        com_mv = 0
                        if c.x != cor[c.row][c.col][0]:
                            c.x = c.x+1 if c.x < cor[c.row][c.col][0] else c.x-1
                            com_mv = 1
                            if (c.x, c.y) == cor[c.row][c.col]:                                
                                if map[c.row][c.col] != None:
                                    (desti, destj) = map[c.row][c.col]
                                    dest_ch = my_ch[desti][destj]
                                    dest_ch.live = 0
                                    chess_num[dest_ch.color] -= 1
                                map[c.row][c.col] = (com_mv_map[0], com_mv_map[1])
                                turn_id = player_color
                        if c.y != cor[c.row][c.col][1]:
                            c.y = c.y+1 if c.y < cor[c.row][c.col][1] else c.y-1
                            com_mv = 1  
                            if (c.x, c.y) == cor[c.row][c.col]:                                
                                if map[c.row][c.col] != None:
                                    (desti, destj) = map[c.row][c.col]
                                    dest_ch = my_ch[desti][destj]
                                    dest_ch.live = 0
                                    chess_num[dest_ch.color] -= 1
                                map[c.row][c.col] = (com_mv_map[0], com_mv_map[1])
                                turn_id = player_color
                        if 1 == com_mv:
                            no_move = 0
                            c.draw(screen, chess_back)
                            com_mv = 0
                if 1 == no_move:
                    turn_id = player_color
            
            chess_ai()
            
            if selected_c != None:
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