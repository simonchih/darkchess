import random, os
import math
import time 
import pygame
import copy
from pygame.locals import *
from sys import exit
from chess import *

background_image_filename = 'Image/SHEET.gif'
image_new        = 'Image/shield-and-swords.gif'
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

image_chess_bks = 'Image/BKS.gif'
image_chess_bas = 'Image/BAS.gif'
image_chess_bbs = 'Image/BBS.gif'
image_chess_brs = 'Image/BRS.gif'
image_chess_bns = 'Image/BNS.gif'
image_chess_bcs = 'Image/BCS.gif'
image_chess_bps = 'Image/BPS.gif'
image_chess_rks = 'Image/RKS.gif'
image_chess_ras = 'Image/RAS.gif'
image_chess_rbs = 'Image/RBS.gif'
image_chess_rrs = 'Image/RRS.gif'
image_chess_rns = 'Image/RNS.gif'
image_chess_rcs = 'Image/RCS.gif'
image_chess_rps = 'Image/RPS.gif'


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
new_game   = pygame.image.load(image_new).convert()
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

chess_bks = pygame.image.load(image_chess_bks).convert()
chess_bas = pygame.image.load(image_chess_bas).convert()
chess_bbs = pygame.image.load(image_chess_bbs).convert()
chess_brs = pygame.image.load(image_chess_brs).convert()
chess_bns = pygame.image.load(image_chess_bns).convert()
chess_bcs = pygame.image.load(image_chess_bcs).convert()
chess_bps = pygame.image.load(image_chess_bps).convert()
chess_rks = pygame.image.load(image_chess_rks).convert()
chess_ras = pygame.image.load(image_chess_ras).convert()
chess_rbs = pygame.image.load(image_chess_rbs).convert()
chess_rrs = pygame.image.load(image_chess_rrs).convert()
chess_rns = pygame.image.load(image_chess_rns).convert()
chess_rcs = pygame.image.load(image_chess_rcs).convert()
chess_rps = pygame.image.load(image_chess_rps).convert()

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
new_game_iconi = 440
new_game_iconj = 13

player_win = 0
player_first = 0
first = 1
# turn_id, 0:black, 1:red, 2:process_moving
turn_id = 0
player_color = 0
com_color = 1
max_value = 0
max_cor = None
open_score = None

chtemp = chess(0, 0, 0, (0, 0), (0, 0), chess_back.get_size(), chess_back, chess_back)
main_chess = [[chtemp, chtemp, chtemp, chtemp, chtemp, chtemp, chtemp, chtemp], [chtemp, chtemp, chtemp, chtemp, chtemp, chtemp, chtemp, chtemp], [chtemp, chtemp, chtemp, chtemp, chtemp, chtemp, chtemp, chtemp], [chtemp, chtemp, chtemp, chtemp, chtemp, chtemp, chtemp, chtemp]]
chess_index = [0] * 32
main_map = [[(0,0)]*8, [(0,0)]*8, [(0,0)]*8, [(0,0)]*8]
cor = [[(0,0)]*8, [(0,0)]*8, [(0,0)]*8, [(0,0)]*8]
mark = [[0]*8, [0]*8, [0]*8, [0]*8]
king_live = [1, 1]
chess_num = [16, 16]
com_mv_map = [0, 0]
back_num = 32

def index_to_chess_select(index):
    if 0 <= index < 5:
        return chess_bps
    elif index < 7:
        return chess_bcs
    elif index < 9:
        return chess_bns
    elif index < 11:
        return chess_brs
    elif index < 13:
        return chess_bbs
    elif index < 15:
        return chess_bas
    elif 15 == index:
        return chess_bks
    elif 16 <= index < 21:
        return chess_rps
    elif index < 23:
        return chess_rcs
    elif index < 25:
        return chess_rns
    elif index < 27:
        return chess_rrs
    elif index < 29:
        return chess_rbs
    elif index < 31:
        return chess_ras
    elif 31 == index:
        return chess_rks

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
            if ch.back < 1 and 1 == ch.live:
                ch.possible_move, a_map, my_chess= collect_possible_move(ch.row, ch.col, a_map, my_chess)
    return a_map, my_chess
    
def collect_possible_move(i, j, a_map, my_chess):
    
    pm = []
    ncor = near(i,j)
    for nc in ncor:
        if None == a_map[nc[0]][nc[1]]:
            pm.append(nc)
        elif a_map[i][j] != None: #a_map[i][j] != None and a_map[nc[0]][nc[1]] != None 
            if my_chess[a_map[nc[0]][nc[1]][0]][a_map[nc[0]][nc[1]][1]].color != my_chess[a_map[i][j][0]][a_map[i][j][1]].color and my_chess[a_map[nc[0]][nc[1]][0]][a_map[nc[0]][nc[1]][1]].back < 1:
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

def eat_by_bomb(org, a_map, my_chess):
    (i, j) = org
    jump = 0
    for ii in range(i-1, -1, -1):
        if 1 == jump and a_map[ii][j] != None:
            if 1 == my_chess[a_map[ii][j][0]][a_map[ii][j][1]].back or my_chess[a_map[ii][j][0]][a_map[ii][j][1]].color == my_chess[a_map[i][j][0]][a_map[i][j][1]].color:
                break
            elif 2 == my_chess[a_map[ii][j][0]][a_map[ii][j][1]].value:
                return 1
        if a_map[ii][j] != None:
            jump = 1
    jump = 0
    for ii in range(i+1, 4, 1):
        if 1 == jump and a_map[ii][j] != None:
            if 1 == my_chess[a_map[ii][j][0]][a_map[ii][j][1]].back or my_chess[a_map[ii][j][0]][a_map[ii][j][1]].color == my_chess[a_map[i][j][0]][a_map[i][j][1]].color:
                break
            elif 2 == my_chess[a_map[ii][j][0]][a_map[ii][j][1]].value:
                return 1
        if a_map[ii][j] != None:
            jump = 1
    jump = 0
    for jj in range(j-1, -1, -1):
        if 1 == jump and a_map[i][jj] != None:
            if 1 == my_chess[a_map[i][jj][0]][a_map[i][jj][1]].back or my_chess[a_map[i][jj][0]][a_map[i][jj][1]].color == my_chess[a_map[i][j][0]][a_map[i][j][1]].color:
                break
            elif 2 == my_chess[a_map[i][jj][0]][a_map[i][jj][1]].value:
                return 1
        if a_map[i][jj] != None:
            jump = 1
    jump = 0
    for jj in range(j+1, 8, 1):
        if 1 == jump and a_map[i][jj] != None:
            if 1 == my_chess[a_map[i][jj][0]][a_map[i][jj][1]].back or my_chess[a_map[i][jj][0]][a_map[i][jj][1]].color == my_chess[a_map[i][j][0]][a_map[i][j][1]].color:
                break
            elif 2 == my_chess[a_map[i][jj][0]][a_map[i][jj][1]].value:
                return 1
        if a_map[i][jj] != None:
            jump = 1
    return 0
    
    
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
    (i, j) = (None, None)
    for k in range(0, 10):
        if 1 == check_back_exist(a_map, my_chess, back_mark):
            (y, x) = random_select_back_chess(a_map, my_chess, back_mark)
            if 0 == eat_by_bomb((y, x), a_map, my_chess):
                near_cor = near(y, x)
                n = 0
                for kk in near_cor:
                    (ni, nj) = kk
                    if a_map[ni][nj] != None:
                        an = a_map[ni][nj]
                        if my_chess[an[0]][an[1]].back < 1:
                            n = 1
                            break
                if 0 == n:
                    (i, j) = (y, x)
                else:
                    back_mark[y][x] = 1
            else:
                back_mark[y][x] = 1
        else:
            break
    if (i, j) != (None, None):
        return (i, j)
    else:
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
    global main_chess
    global main_map
    global player_color
    global com_color
    global player_first
    global player_win
    global back_num
    
    pygame.display.update()
    
    if 0 == player_first and  1 == first:
        i = random.randint(0, 3) 
        j = random.randint(0, 7) 
        turn_id = main_chess[i][j].color
        main_chess[i][j].back = -1
        back_num -= 1
        com_color = turn_id
        player_color = 1 - com_color
        first = 0
    elif turn_id == com_color and 0 == first:
        main_chess = clean_back_n1_to_0(main_chess)
        org, dest, score = com_think(main_map, main_chess)
        print 'org', org, 'dest', dest, 'score', score, 'op score', open_score
        if 0 == back_num and 1 == cant_move(main_map, main_chess, com_color):
            player_win = 1
        if back_num > 0:
            if open_score != None:
                if org == None or score >= open_score - 4:
                    dest = select_back_chess(main_map, main_chess)
                    sound_click.play()
                    main_chess[main_map[dest[0]][dest[1]][0]][main_map[dest[0]][dest[1]][1]].back = -1
                    back_num -= 1 
                elif score == open_score:
                    if score >= 0:
                        dest = select_back_chess(main_map, main_chess)
                        sound_click.play()
                        main_chess[main_map[dest[0]][dest[1]][0]][main_map[dest[0]][dest[1]][1]].back = -1
                        back_num -= 1
                    else:
                        main_map, main_chess = move_s(org, dest, main_map, main_chess)
                else:
                    main_map, main_chess = move_s(org, dest, main_map, main_chess)
            else:
                main_map, main_chess = move_s(org, dest, main_map, main_chess)
        elif 0 == player_win:
            main_map, main_chess = move_s(org, dest, main_map, main_chess) 
   
    if turn_id == com_color:
        turn_id = 2
        

def move_max_value(orgx, orgy, destx, desty, my_chess, a_map, org_value, owner_color, i, j):
    global max_value
    global mark
    global max_cor
    
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
    
    if a_map[i][j] != None:
        if 7 == org_value:
            if 1 == my_chess[a_map[i][j][0]][a_map[i][j][1]].value:
                max_value = 8
                max_cor = (i, j)
                return
            elif max_value < my_chess[a_map[i][j][0]][a_map[i][j][1]].value:
                max_value = my_chess[a_map[i][j][0]][a_map[i][j][1]].value
                max_cor = (i, j)
                return
        elif 1 == org_value:
            if 7 == my_chess[a_map[i][j][0]][a_map[i][j][1]].value:
                max_value = 9
                max_cor = (i, j)
                return
            elif max_value < my_chess[a_map[i][j][0]][a_map[i][j][1]].value:
                max_value = my_chess[a_map[i][j][0]][a_map[i][j][1]].value
                max_cor = (i, j)
                return
        elif max_value < my_chess[a_map[i][j][0]][a_map[i][j][1]].value:
            max_value = my_chess[a_map[i][j][0]][a_map[i][j][1]].value
            max_cor = (i, j)
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

def caca(org, dest, my_chess, a_map, owner_color):        
    if org == None:
        return 0
    
    (orgy, orgx) = org
    (desty, destx) = dest
    
    m = a_map[orgy][orgx]
    if m == None:
        return 0
    elif 2 == my_chess[m[0]][m[1]].value:
        return 0
    
    if desty-1 >= 0 and destx-1 >=0:
        n = a_map[desty-1][destx-1]
        if n == None:
            return 0
        else:
            mc = my_chess[n[0]][n[1]]
            if 0 == mc.live or 1 == mc.back:
                return 0
            elif 7 == my_chess[m[0]][m[1]].value and 1 == my_chess[n[0]][n[1]].value and my_chess[n[0]][n[1]].color != my_chess[m[0]][m[1]].color:
                return 0 
            elif my_chess[n[0]][n[1]].color != my_chess[m[0]][m[1]].color and my_chess[n[0]][n[1]].value == my_chess[m[0]][m[1]].value:
                return 1
            elif 1 == my_chess[m[0]][m[1]].value and 7 == my_chess[n[0]][n[1]].value and my_chess[n[0]][n[1]].color != my_chess[m[0]][m[1]].color:
                return 1
    if desty-1 >= 0 and destx+1 <=7:
        n = a_map[desty-1][destx+1]
        if n == None:
            return 0
        else:
            mc = my_chess[n[0]][n[1]]
            if 0 == mc.live or 1 == mc.back:
                return 0
            elif 7 == my_chess[m[0]][m[1]].value and 1 == my_chess[n[0]][n[1]].value and my_chess[n[0]][n[1]].color != my_chess[m[0]][m[1]].color:
                return 0
            elif my_chess[n[0]][n[1]].color != my_chess[m[0]][m[1]].color and my_chess[n[0]][n[1]].value == my_chess[m[0]][m[1]].value:
                return 1
            elif 1 == my_chess[m[0]][m[1]].value and 7 == my_chess[n[0]][n[1]].value and my_chess[n[0]][n[1]].color != my_chess[m[0]][m[1]].color:
                return 1
    if desty+1 <= 3 and destx-1 >= 0:
        n = a_map[desty+1][destx-1]
        if n == None:
            return 0
        else:
            mc = my_chess[n[0]][n[1]]
            if 0 == mc.live or 1 == mc.back:
                return 0
            elif 7 == my_chess[m[0]][m[1]].value and 1 == my_chess[n[0]][n[1]].value and my_chess[n[0]][n[1]].color != my_chess[m[0]][m[1]].color:
                return 0
            elif my_chess[n[0]][n[1]].color != my_chess[m[0]][m[1]].color and my_chess[n[0]][n[1]].value == my_chess[m[0]][m[1]].value:
                return 1
            elif 1 == my_chess[m[0]][m[1]].value and 7 == my_chess[n[0]][n[1]].value and my_chess[n[0]][n[1]].color != my_chess[m[0]][m[1]].color:
                return 1
    if desty+1 <= 3 and destx+1 <= 7:
        n = a_map[desty+1][destx+1]
        if n == None:
            return 0
        else:
            mc = my_chess[n[0]][n[1]]
            if 0 == mc.live or 1 == mc.back:
                return 0
            elif 7 == my_chess[m[0]][m[1]].value and 1 == my_chess[n[0]][n[1]].value and my_chess[n[0]][n[1]].color != my_chess[m[0]][m[1]].color:
                return 0
            elif my_chess[n[0]][n[1]].color != my_chess[m[0]][m[1]].color and my_chess[n[0]][n[1]].value == my_chess[m[0]][m[1]].value:
                return 1
            elif 1 == my_chess[m[0]][m[1]].value and 7 == my_chess[n[0]][n[1]].value and my_chess[n[0]][n[1]].color != my_chess[m[0]][m[1]].color:
                return 1
        
        
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
        else:
            mc = my_chess[n[0]][n[1]]
            if 0 == mc.live or 1 == mc.back:
                return 0
            elif 7 == my_chess[m[0]][m[1]].value and 1 == my_chess[n[0]][n[1]].value and my_chess[n[0]][n[1]].color != my_chess[m[0]][m[1]].color:
                return 0
            elif my_chess[n[0]][n[1]].color != my_chess[m[0]][m[1]].color and my_chess[n[0]][n[1]].value == my_chess[m[0]][m[1]].value:
                return 1
            elif 1 == my_chess[m[0]][m[1]].value and 7 == my_chess[n[0]][n[1]].value and my_chess[n[0]][n[1]].color != my_chess[m[0]][m[1]].color:
                return 1
    if orgy+2 <= 3:
        n = a_map[orgy+2][orgx]
        if n == None:
            return 0
        else:
            mc = my_chess[n[0]][n[1]]
            if 0 == mc.live or 1 == mc.back:
                return 0
            elif 7 == my_chess[m[0]][m[1]].value and 1 == my_chess[n[0]][n[1]].value and my_chess[n[0]][n[1]].color != my_chess[m[0]][m[1]].color:
                return 0
            elif my_chess[n[0]][n[1]].color != my_chess[m[0]][m[1]].color and my_chess[n[0]][n[1]].value == my_chess[m[0]][m[1]].value:
                return 1
            elif 1 == my_chess[m[0]][m[1]].value and 7 == my_chess[n[0]][n[1]].value and my_chess[n[0]][n[1]].color != my_chess[m[0]][m[1]].color:
                return 1
    if orgx-2 >= 0:
        n = a_map[orgy][orgx-2]
        if n == None:
            return 0
        else:
            mc = my_chess[n[0]][n[1]]
            if 0 == mc.live or 1 == mc.back:
                return 0
            elif 7 == my_chess[m[0]][m[1]].value and 1 == my_chess[n[0]][n[1]].value and my_chess[n[0]][n[1]].color != my_chess[m[0]][m[1]].color:
                return 0
            elif my_chess[n[0]][n[1]].color != my_chess[m[0]][m[1]].color and my_chess[n[0]][n[1]].value == my_chess[m[0]][m[1]].value:
                return 1
            elif 1 == my_chess[m[0]][m[1]].value and 7 == my_chess[n[0]][n[1]].value and my_chess[n[0]][n[1]].color != my_chess[m[0]][m[1]].color:
                return 1
    if orgx+2 <= 7:
        n = a_map[orgy][orgx+2]
        if n == None:
            return 0
        else:
            mc = my_chess[n[0]][n[1]]
            if 0 == mc.live or 1 == mc.back:
                return 0
            elif 7 == my_chess[m[0]][m[1]].value and 1 == my_chess[n[0]][n[1]].value and my_chess[n[0]][n[1]].color != my_chess[m[0]][m[1]].color:
                return 0
            elif my_chess[n[0]][n[1]].color != my_chess[m[0]][m[1]].color and my_chess[n[0]][n[1]].value == my_chess[m[0]][m[1]].value:
                return 1
            elif 1 == my_chess[m[0]][m[1]].value and 7 == my_chess[n[0]][n[1]].value and my_chess[n[0]][n[1]].color != my_chess[m[0]][m[1]].color:
                return 1
    if orgy-1 >= 0 and orgx-1 >=0:
        n = a_map[orgy-1][orgx-1]
        if n == None:
            return 0
        else:
            mc = my_chess[n[0]][n[1]]
            if 0 == mc.live or 1 == mc.back:
                return 0
            elif 7 == my_chess[m[0]][m[1]].value and 1 == my_chess[n[0]][n[1]].value and my_chess[n[0]][n[1]].color != my_chess[m[0]][m[1]].color:
                return 0
            elif my_chess[n[0]][n[1]].color != my_chess[m[0]][m[1]].color and my_chess[n[0]][n[1]].value == my_chess[m[0]][m[1]].value:
                return 1
            elif 1 == my_chess[m[0]][m[1]].value and 7 == my_chess[n[0]][n[1]].value and my_chess[n[0]][n[1]].color != my_chess[m[0]][m[1]].color:
                return 1
    if orgy-1 >= 0 and orgx+1 <=7:
        n = a_map[orgy-1][orgx+1]
        if n == None:
            return 0
        else:
            mc = my_chess[n[0]][n[1]]
            if 0 == mc.live or 1 == mc.back:
                return 0
            elif 7 == my_chess[m[0]][m[1]].value and 1 == my_chess[n[0]][n[1]].value and my_chess[n[0]][n[1]].color != my_chess[m[0]][m[1]].color:
                return 0
            elif my_chess[n[0]][n[1]].color != my_chess[m[0]][m[1]].color and my_chess[n[0]][n[1]].value == my_chess[m[0]][m[1]].value:
                return 1
            elif 1 == my_chess[m[0]][m[1]].value and 7 == my_chess[n[0]][n[1]].value and my_chess[n[0]][n[1]].color != my_chess[m[0]][m[1]].color:
                return 1
    if orgy+1 <= 3 and orgx-1 >= 0:
        n = a_map[orgy+1][orgx-1]
        if n == None:
            return 0
        else:
            mc = my_chess[n[0]][n[1]]
            if 0 == mc.live or 1 == mc.back:
                return 0
            elif 7 == my_chess[m[0]][m[1]].value and 1 == my_chess[n[0]][n[1]].value and my_chess[n[0]][n[1]].color != my_chess[m[0]][m[1]].color:
                return 0
            elif my_chess[n[0]][n[1]].color != my_chess[m[0]][m[1]].color and my_chess[n[0]][n[1]].value == my_chess[m[0]][m[1]].value:
                return 1
            elif 1 == my_chess[m[0]][m[1]].value and 7 == my_chess[n[0]][n[1]].value and my_chess[n[0]][n[1]].color != my_chess[m[0]][m[1]].color:
                return 1
    if orgy+1 <= 3 and orgx+1 <= 7:
        n = a_map[orgy+1][orgx+1]
        if n == None:
            return 0
        else:
            mc = my_chess[n[0]][n[1]]
            if 0 == mc.live or 1 == mc.back:
                return 0
            elif 7 == my_chess[m[0]][m[1]].value and 1 == my_chess[n[0]][n[1]].value and my_chess[n[0]][n[1]].color != my_chess[m[0]][m[1]].color:
                return 0
            elif my_chess[n[0]][n[1]].color != my_chess[m[0]][m[1]].color and my_chess[n[0]][n[1]].value == my_chess[m[0]][m[1]].value:
                return 1
            elif 1 == my_chess[m[0]][m[1]].value and 7 == my_chess[n[0]][n[1]].value and my_chess[n[0]][n[1]].color != my_chess[m[0]][m[1]].color:
                return 1

def scan_king(my_chess):
    global king_live
    
    for chr in my_chess:
        for ch in chr:
            if 7 == ch.value:
                king_live[ch.color] = ch.live
                
def move_score(org, dest, my_chess, a_map, owner_color):
    
    global king_live
    global max_value
    global mark
    global max_cor
    
    (orgy, orgx) = org
    (desty, destx) = dest
    
    scan_king(my_chess)
    
    if a_map[desty][destx] == None:
        if owner_color == player_color:
            return 0
        if  2 == my_chess[a_map[orgy][orgx][0]][a_map[orgy][orgx][1]].value:
            return 0
        
        max_value = 0
        max_cor = None
        mark = [[0]*8, [0]*8, [0]*8, [0]*8]
        org_value = my_chess[a_map[orgy][orgx][0]][a_map[orgy][orgx][1]].value
        if 1 == near2_have_same_value(org, my_chess, a_map, owner_color):
            return -0.001
        elif 1 == caca(org, dest, my_chess, a_map, owner_color):
            return org_value+0.001
        move_max_value(orgx, orgy, destx, desty, my_chess, a_map, org_value, my_chess[a_map[orgy][orgx][0]][a_map[orgy][orgx][1]].color, desty, destx)
        print 'max_value', max_value, 'max_cor', max_cor, 'org', org, 'dest', dest, 'owner_color', owner_color
        print 'mark', mark
        if 8 == max_value:
            return org_value/2
        elif 9 == max_value:
            if max_cor != None:
                return 7.0 - 0.1*(abs(max_cor[0]-orgy)+abs(max_cor[1]-orgx))
            else:
                return 7
        elif max_value > org_value:
            return org_value/2
        else:
            if max_cor != None:
                return (float)(max_value) - 0.1*(abs(max_cor[0]-orgy)+abs(max_cor[1]-orgx))
            else:
                return max_value
    
    elif 1 == my_chess[a_map[desty][destx][0]][a_map[desty][destx][1]].live:
        #print 'owner_color', owner_color, 'org', org, 'dest', dest, 'eating score', eating_value_to_score(my_chess[a_map[desty][destx][0]][a_map[desty][destx][1]].value, king_live, my_chess[a_map[orgy][orgx][0]][a_map[orgy][orgx][1]].color)
        return eating_value_to_score(my_chess[a_map[desty][destx][0]][a_map[desty][destx][1]].value, king_live, my_chess[a_map[orgy][orgx][0]][a_map[orgy][orgx][1]].color)

def move_s(org, dest, a_map, a_ch):
    global cor
    global com_mv_map
    
    (orgi, orgj) = org
    (desti, destj) = dest
    
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
                    #print (ch.row, ch.col), pm, 'will dead pity', will_dead_pity((ch.row, ch.col), pm, a_ch, a_map, com_color)
                    if 0 == will_dead_pity((ch.row, ch.col), pm, a_ch, a_map, com_color):
                        score = sc - move_score((ch.row, ch.col), pm, a_ch, a_map, com_color)
                    else:
                        score = sc + 320
                    m.append(((ch.row, ch.col), pm, score))
                    #print 'm', m
                    if score < max_score:
                        max_score = score
                        org = (ch.row, ch.col)
                        dest = pm   
    if len(m) > 1:
        mf = []
        for mm in m:
            m2 = []
            m3 = []
            m4 = []
            m2, a2_map, a2_ch= one_turn(a_map, a_ch, mm, player_color, mm[0], mm[1], mm[2], 0.9)
            if m2:
                m2 = sorted(m2, key=lambda s:s[4])
                if mm[0] == mm[1]:
                    open_score = m2[-1][4]
                #if m2[-1][4] == mm[2]:
                #    a2_map = a_map
                #    a2_ch = a_ch
                #    m2[-1][2] = None
                mf.append([mm[0], mm[1], m2[-1][4]])
                if mm[0] == mm[1]:
                    continue
                m3, a3_map, a3_ch= one_turn(a2_map, a2_ch, mm, com_color, m2[-1][2], m2[-1][3], m2[-1][4], 0.8)
                if m3:
                    m3 = sorted(m3, key=lambda s:s[4])
                    if m3[0][2] == None:
                        continue
                    m4, a4_map, a4_ch= one_turn(a3_map, a3_ch, mm, player_color, m3[0][2], m3[0][3], m3[0][4], 0.7)
                    if m4:
                        m4 = sorted(m4, key=lambda s:s[4])
                        mf.append([mm[0], mm[1], m4[-1][4]])
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
    if nexti != None and nextj != None:
        af_map, af_ch = move(nexti, nextj, af_map, af_ch)
        af_map, af_ch = all_chess_move(af_map, af_ch)
    
    if owner_color == player_color and 1 == cant_move(af_map, af_ch, player_color):
        m2.append([mm[0], mm[1], None, None, sc])
        return m2, af_map, af_ch

    if back_num > 0:
        m2.append([mm[0], mm[1], None, None, sc])
    for chr in af_ch:
        for ch in chr:
            if ch.color == owner_color and 1 == ch.live and ch.back < 1:
                #print 'owner_color', owner_color, 'ch.row', ch.row, 'ch.col', ch.col, 'pm', ch.possible_move 
                for pm in ch.possible_move:
                    if 0 == will_dead_pity((ch.row, ch.col), pm, af_ch, af_map, owner_color):
                        #print (ch.row, ch.col), pm, 'will dead pity', will_dead_pity((ch.row, ch.col), pm, af_ch, af_map, owner_color)
                        if owner_color == player_color:
                            score = sc + div * move_score((ch.row, ch.col), pm, af_ch, af_map, player_color)
                        else:
                            score = sc - div * move_score((ch.row, ch.col), pm, af_ch, af_map, com_color)
                    else:
                        #print (ch.row, ch.col), pm, 'will dead pity', will_dead_pity((ch.row, ch.col), pm, af_ch, af_map, owner_color)
                        if owner_color == com_color:
                            score = sc + 320
                        else:
                            score = sc - 8
                    
                    m2.append([mm[0], mm[1], (ch.row, ch.col), pm, score])
                    
    return m2, af_map, af_ch

def will_dead(org, a_ch, opp_color):
    for chr in a_ch:
        for ch in chr:
            if 1 == ch.live and ch.back < 1 and ch.color == opp_color: 
                for pm in ch.possible_move:
                    if pm == org:
                        return 1
    return 0
    
def will_dead_pity(nexti, nextj, a_ch, a_map, owner_color):
    if 1 == will_dead(nexti, a_ch, 1-owner_color):
        return 0
    
    (y, x) = nexti
    a = a_map[y][x]

    if nextj != None:
        (ii, jj) = nextj
        b = a_map[ii][jj]
        if b != None:
            if 2 == a_ch[a[0]][a[1]].value:
                if a_ch[b[0]][b[1]].value > 4:
                    return 0
            if a_ch[b[0]][b[1]].value == a_ch[a[0]][a[1]].value:
                return 0
    
    af_map = copy.deepcopy(a_map)
    af_ch = copy.deepcopy(a_ch)
    af_map, af_ch = move(nexti, nextj, af_map, af_ch)
    af_map, af_ch = all_chess_move(af_map, af_ch)
    opp_color = 1 - owner_color

    pity = 0
    i2 = None
    j2 = None
    
    for chr in af_ch:
        if 1 == pity:
            break
        for ch in chr:
            if ch.color == opp_color and 1 == ch.live:
                for pm in ch.possible_move:
                    if pm == nextj:
                        if b == None:
                            pity = 1
                            break
                        elif eating_value_to_score(a_ch[a[0]][a[1]].value, king_live, 1-owner_color) > eating_value_to_score(a_ch[b[0]][b[1]].value, king_live, owner_color):
                            i2 = (ch.row, ch.col)
                            j2 = pm
                            pity = 1
                            break
    
    if i2!= None and j2!= None:                    
        af2_map = copy.deepcopy(af_map)
        af2_ch = copy.deepcopy(af_ch)
        af2_map, af2_ch = move(i2, j2, af2_map, af2_ch)
        af2_map, af2_ch = all_chess_move(af2_map, af2_ch)
        
        for chr in af2_ch:
            if 0 == pity:
                break
            for ch in chr:
                if ch.color == owner_color and 1 == ch.live:
                    for pm in ch.possible_move:
                        if pm == j2:
                            pity = 0
                            break    
    return pity
        
def eating_value_to_score(value, king, owner_color):
    if 1 == value:
        if 1 == king[owner_color]:
            return 19
        else:
            return 15
    elif 2 == value:
        return 79
    elif 3 == value:
        return 18
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

def clean_back_n1_to_0(a_ch):
    for chr in a_ch:
        for ch in chr:
            if -1 == ch.back:
                ch.back = 0
    return a_ch
    
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
    global main_chess
    global main_map
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
                ch = chess(chess_index[8*i+j],index_to_color(chess_index[8*i+j]), index_to_chess_value(chess_index[8*i+j]), (cstart_x+j*chess_back.get_width(),cstart_y+i*chess_back.get_height()), (i, j), chess_back.get_size(), index_to_chess_surface(chess_index[8*i+j]), index_to_chess_select(chess_index[8*i+j]))
                main_chess[i][j] = ch
                cor[i][j] = (ch.x, ch.y)
                main_map[i][j] = (i, j)
        for i in range(0, 4):
            for j in range(0, 4):
                ch = chess(chess_index[8*i+4+j],index_to_color(chess_index[8*i+4+j]), index_to_chess_value(chess_index[8*i+4+j]), (cstart_x2+j*chess_back.get_width(),cstart_y2+i*chess_back.get_height()), (i, 4+j), chess_back.get_size(), index_to_chess_surface(chess_index[8*i+4+j]), index_to_chess_select(chess_index[8*i+4+j]))
                main_chess[i][4+j] = ch
                cor[i][4+j] = (ch.x, ch.y)
                main_map[i][4+j] = (i, 4+j)
        
        while 0 == player_win:
            if 1 == game_start:
                sound_new.play()
                game_start = 0

            screen.blit(background, (0,0))
            screen.blit(new_game, (new_game_iconi, new_game_iconj))
            
            display_font()
            for cr in main_chess:
                for c in cr:
                    c.draw(screen, chess_back)
                       
            no_move = 1
            if 2 == turn_id:
                for cr in main_chess:
                    for c in cr:
                        com_mv = 0
                        if c.x != cor[c.row][c.col][0]:
                            c.x = c.x+1 if c.x < cor[c.row][c.col][0] else c.x-1
                            com_mv = 1
                            if (c.x, c.y) == cor[c.row][c.col]:                                
                                if main_map[c.row][c.col] != None:
                                    (desti, destj) = main_map[c.row][c.col]
                                    dest_ch = main_chess[desti][destj]
                                    dest_ch.live = 0
                                    chess_num[dest_ch.color] -= 1
                                main_map[c.row][c.col] = (com_mv_map[0], com_mv_map[1])
                                turn_id = player_color
                        if c.y != cor[c.row][c.col][1]:
                            c.y = c.y+1 if c.y < cor[c.row][c.col][1] else c.y-1
                            com_mv = 1  
                            if (c.x, c.y) == cor[c.row][c.col]:                                
                                if main_map[c.row][c.col] != None:
                                    (desti, destj) = main_map[c.row][c.col]
                                    dest_ch = main_chess[desti][destj]
                                    dest_ch.live = 0
                                    chess_num[dest_ch.color] -= 1
                                main_map[c.row][c.col] = (com_mv_map[0], com_mv_map[1])
                                turn_id = player_color
                        if 1 == com_mv:
                            no_move = 0
                            c.draw(screen, chess_back)
                            com_mv = 0
                if 1 == no_move:
                    turn_id = player_color
            
            if selected_c != None:
                selected_c.move()
                selected_c.draw(screen, chess_back)
            
            chess_ai()
            
            if turn_id == player_color:
                if 0 == back_num and 1 == cant_move(main_map, main_chess, player_color):
                    player_win = -1
                
                for event in pygame.event.get():
                    if event.type == QUIT:
                        exit()
                    elif event.type == pygame.MOUSEBUTTONDOWN and turn_id == player_color:
                        (mouseX, mouseY) = pygame.mouse.get_pos()
                        if new_game_iconi < mouseX < new_game_iconi + new_game.get_width() and new_game_iconj < mouseY < new_game_iconj + new_game.get_height():
                            player_win = -1
                        main_map, main_chess = all_chess_move(main_map, main_chess)
                        sound_click.play()
                        click_once = 0
                        for chr in main_chess:
                            for chc in chr:
                                if 0 == click_once:
                                    ch_index = chc.click((mouseX, mouseY))
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
                                        elif ch_index != -1 and 0 == first:
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
                                    if main_map[pm[0]][pm[1]] != None:
                                        main_chess[main_map[pm[0]][pm[1]][0]][main_map[pm[0]][pm[1]][1]].live = 0
                                        chess_num[main_chess[main_map[pm[0]][pm[1]][0]][main_map[pm[0]][pm[1]][1]].color] -= 1
                                        sound_capture.play()
                                    else:
                                        sound_move.play()
                                    main_map[pm[0]][pm[1]] = main_map[selected_c.row][selected_c.col]
                                    main_map[selected_c.row][selected_c.col] = None
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
                main_map, main_chess = all_chess_move(main_map, main_chess)
                moving = 0
            
            if selected_c != None:
                (mouseX, mouseY) = pygame.mouse.get_pos()
                dx = mouseX - selected_c.x
                dy = mouseY - selected_c.y
                dx -= selected_c.size[0]/2
                dy -= selected_c.size[1]/2
                selected_c.angle = 0.5*math.pi + math.atan2(dy, dx)
                selected_c.speed = math.hypot(dx, dy) * 0.1
            
            if 0 == chess_num[player_color]:
                player_win = -1
                
            elif 0 == chess_num[com_color]:
                player_win = 1
            
            if 1 == player_win:
                screen.blit(background, (0,0))
                display_font()
                for cr in main_chess:
                    for c in cr:
                        c.draw(screen, chess_back)
                sound_win.play()
                pygame.display.update()
                time.sleep(5)
            elif -1 == player_win:
                screen.blit(background, (0,0))
                display_font()
                for cr in main_chess:
                    for c in cr:
                        c.draw(screen, chess_back)
                sound_loss.play()
                pygame.display.update()
                time.sleep(5)
            
            pygame.display.update()
            
    exit()

if __name__ == "__main__":
    main()