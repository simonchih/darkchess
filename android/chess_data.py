import pygame

cstart_x = 68
cstart_y = 102
cstart_x2 = 520
cstart_y2 = 102

SCREEN_SIZE = (1042, 626) 
pygame.init()

pygame.display.set_icon(pygame.image.load("Image/darkchess_default.png"))
screen = pygame.display.set_mode(SCREEN_SIZE, 0, 32)#SCREEN_SIZE, FULLSCREEN, 32)
pygame.display.set_caption("Taiwan Blind Chess")

image_chess_back = 'Image/back.gif'

image_chess_bk = 'Image/BK.GIF'
image_chess_ba = 'Image/BA.GIF'
image_chess_bb = 'Image/BB.GIF'
image_chess_br = 'Image/BR.GIF'
image_chess_bn = 'Image/BN.GIF'
image_chess_bc = 'Image/BC.GIF'
image_chess_bp = 'Image/BP.GIF'
image_chess_rk = 'Image/RK.GIF'
image_chess_ra = 'Image/RA.GIF'
image_chess_rb = 'Image/RB.GIF'
image_chess_rr = 'Image/RR.GIF'
image_chess_rn = 'Image/RN.GIF'
image_chess_rc = 'Image/RC.GIF'
image_chess_rp = 'Image/RP.GIF'

image_chess_bks = 'Image/BKS.GIF'
image_chess_bas = 'Image/BAS.GIF'
image_chess_bbs = 'Image/BBS.GIF'
image_chess_brs = 'Image/BRS.GIF'
image_chess_bns = 'Image/BNS.GIF'
image_chess_bcs = 'Image/BCS.GIF'
image_chess_bps = 'Image/BPS.GIF'
image_chess_rks = 'Image/RKS.GIF'
image_chess_ras = 'Image/RAS.GIF'
image_chess_rbs = 'Image/RBS.GIF'
image_chess_rrs = 'Image/RRS.GIF'
image_chess_rns = 'Image/RNS.GIF'
image_chess_rcs = 'Image/RCS.GIF'
image_chess_rps = 'Image/RPS.GIF'

chess_back = pygame.image.load(image_chess_back).convert_alpha()

chess_bk = pygame.image.load(image_chess_bk).convert_alpha()
chess_ba = pygame.image.load(image_chess_ba).convert_alpha()
chess_bb = pygame.image.load(image_chess_bb).convert_alpha()
chess_br = pygame.image.load(image_chess_br).convert_alpha()
chess_bn = pygame.image.load(image_chess_bn).convert_alpha()
chess_bc = pygame.image.load(image_chess_bc).convert_alpha()
chess_bp = pygame.image.load(image_chess_bp).convert_alpha()
chess_rk = pygame.image.load(image_chess_rk).convert_alpha()
chess_ra = pygame.image.load(image_chess_ra).convert_alpha()
chess_rb = pygame.image.load(image_chess_rb).convert_alpha()
chess_rr = pygame.image.load(image_chess_rr).convert_alpha()
chess_rn = pygame.image.load(image_chess_rn).convert_alpha()
chess_rc = pygame.image.load(image_chess_rc).convert_alpha()
chess_rp = pygame.image.load(image_chess_rp).convert_alpha()

chess_bks = pygame.image.load(image_chess_bks).convert_alpha()
chess_bas = pygame.image.load(image_chess_bas).convert_alpha()
chess_bbs = pygame.image.load(image_chess_bbs).convert_alpha()
chess_brs = pygame.image.load(image_chess_brs).convert_alpha()
chess_bns = pygame.image.load(image_chess_bns).convert_alpha()
chess_bcs = pygame.image.load(image_chess_bcs).convert_alpha()
chess_bps = pygame.image.load(image_chess_bps).convert_alpha()
chess_rks = pygame.image.load(image_chess_rks).convert_alpha()
chess_ras = pygame.image.load(image_chess_ras).convert_alpha()
chess_rbs = pygame.image.load(image_chess_rbs).convert_alpha()
chess_rrs = pygame.image.load(image_chess_rrs).convert_alpha()
chess_rns = pygame.image.load(image_chess_rns).convert_alpha()
chess_rcs = pygame.image.load(image_chess_rcs).convert_alpha()
chess_rps = pygame.image.load(image_chess_rps).convert_alpha()

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
        
def index_to_color(index):
    if 0 <= index < 16:
        return 0
    else:
        return 1