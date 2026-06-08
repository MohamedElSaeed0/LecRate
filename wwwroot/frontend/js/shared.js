





function injectSharedHTML() {
    
    if (!document.getElementById('toastNotification')) {
        const toast = document.createElement('div');
        toast.id = 'toastNotification';
        toast.className = 'shared-toast';
        toast.innerHTML = `
            <i class="fas fa-check-circle" style="color: var(--success); font-size: 1.5rem;" id="toastIcon"></i>
            <div>
                <h4 style="margin: 0; font-size: 1rem;" id="toastTitle">نجاح</h4>
                <p style="margin: 0; font-size: 0.9rem; color: var(--text-muted);" id="toastMessage">تمت العملية</p>
            </div>
        `;
        document.body.appendChild(toast);
    }

    
    if (!document.getElementById('confirmModal')) {
        const modal = document.createElement('div');
        modal.className = 'confirm-modal-overlay';
        modal.id = 'confirmModal';
        modal.innerHTML = `
            <div class="confirm-modal"
                style="background: white; border-radius: var(--radius-lg); padding: 30px; box-shadow: var(--shadow-xl); max-width: 400px; text-align: center; transform: scale(0.9); transition: all 0.3s; opacity: 0;">
                <div class="confirm-icon-container"
                    style="width: 70px; height: 70px; border-radius: 50%; background: rgba(245, 158, 11, 0.1); color: var(--warning); display: flex; align-items: center; justify-content: center; font-size: 2rem; margin: 0 auto 20px auto;">
                    <i class="fas fa-exclamation-triangle"></i>
                </div>
                <h3 style="margin: 0 0 10px 0; color: var(--text-main); font-size: 1.4rem;">تأكيد الإجراء</h3>
                <p id="confirmMessage" style="color: var(--text-muted); margin-bottom: 25px; line-height: 1.6;">هل أنت متأكد من تنفيذ هذا الإجراء؟</p>
                <div style="display: flex; gap: 15px; justify-content: center;">
                    <button class="btn" id="confirmNo"
                        style="background: var(--bg-main); color: var(--text-main); border: 1px solid var(--border-color); flex: 1; justify-content: center;">إلغاء</button>
                    <button class="btn" id="confirmYes"
                        style="background: var(--danger); color: white; flex: 1; justify-content: center;">نعم، احذف</button>
                </div>
            </div>
        `;
        document.body.appendChild(modal);
    }
}


function showFancyAlert(title, message, type = 'success') {
    const toast = document.getElementById('toastNotification');
    const icon = document.getElementById('toastIcon');
    if (!toast || !icon) return;

    document.getElementById('toastTitle').textContent = title;
    document.getElementById('toastMessage').textContent = message;

    if (type === 'success') {
        toast.style.borderRightColor = 'var(--success)';
        icon.className = 'fas fa-check-circle';
        icon.style.color = 'var(--success)';
    } else if (type === 'danger' || type === 'error') {
        toast.style.borderRightColor = 'var(--danger)';
        icon.className = 'fas fa-times-circle';
        icon.style.color = 'var(--danger)';
    } else if (type === 'warning') {
        toast.style.borderRightColor = 'var(--warning)';
        icon.className = 'fas fa-exclamation-triangle';
        icon.style.color = 'var(--warning)';
    } else {
        toast.style.borderRightColor = 'var(--info)';
        icon.className = 'fas fa-info-circle';
        icon.style.color = 'var(--info)';
    }

    toast.style.bottom = '30px';
    setTimeout(() => {
        toast.style.bottom = '-100px';
    }, 4000);
}


function showConfirm(message, type = 'danger', yesText = null) {
    return new Promise((resolve) => {
        const modalOverlay = document.getElementById('confirmModal');
        const modalContent = modalOverlay.querySelector('.confirm-modal');
        const iconContainer = modalOverlay.querySelector('.confirm-icon-container');
        const icon = iconContainer.querySelector('i');
        const yesBtn = document.getElementById('confirmYes');

        
        if (type === 'danger') {
            iconContainer.style.background = 'rgba(239, 68, 68, 0.1)';
            iconContainer.style.color = 'var(--danger)';
            icon.className = 'fas fa-exclamation-circle';
            yesBtn.style.background = 'var(--danger)';
        } else if (type === 'info') {
            iconContainer.style.background = 'rgba(14, 165, 233, 0.1)';
            iconContainer.style.color = 'var(--info)';
            icon.className = 'fas fa-info-circle';
            yesBtn.style.background = 'var(--primary)';
        } else {
            iconContainer.style.background = 'rgba(245, 158, 11, 0.1)';
            iconContainer.style.color = 'var(--warning)';
            icon.className = 'fas fa-exclamation-triangle';
            yesBtn.style.background = 'var(--primary)';
        }

        
        if (yesText) {
            yesBtn.textContent = yesText;
        } else {
            if (type === 'danger') {
                yesBtn.textContent = 'نعم، احذف';
            } else {
                yesBtn.textContent = 'نعم، موافق';
            }
        }

        document.getElementById('confirmMessage').textContent = message;

        modalOverlay.classList.add('active');
        void modalOverlay.offsetWidth;
        modalContent.style.opacity = '1';
        modalContent.style.transform = 'scale(1)';

        const closeWith = (result) => {
            modalContent.style.opacity = '0';
            modalContent.style.transform = 'scale(0.9)';
            setTimeout(() => {
                modalOverlay.classList.remove('active');
                resolve(result);
            }, 300);
        };

        document.getElementById('confirmYes').onclick = () => closeWith(true);
        document.getElementById('confirmNo').onclick = () => closeWith(false);
    });
}


function animateValue(target, start, end, duration) {
    
    const obj = (typeof target === 'string') ? document.getElementById(target) : target;
    if (!obj) return;
    if (start === end) { obj.textContent = end; return; }

    let startTimestamp = null;
    const step = (timestamp) => {
        if (!startTimestamp) startTimestamp = timestamp;
        const progress = Math.min((timestamp - startTimestamp) / duration, 1);
        obj.textContent = Math.floor(progress * (end - start) + start);
        if (progress < 1) {
            window.requestAnimationFrame(step);
        } else {
            obj.textContent = end;
        }
    };
    window.requestAnimationFrame(step);
}




function setupHeaderUserInfo(options = {}) {
    const fullName = localStorage.getItem('fullName');
    const firstName = localStorage.getItem('firstName');

    const nameEl = options.nameId ? document.getElementById(options.nameId) : null;
    const avatarEl = options.avatarId ? document.getElementById(options.avatarId) : null;
    const welcomeEl = options.welcomeId ? document.getElementById(options.welcomeId) : null;
    const prefix = options.prefix || '';

    const displayName = (fullName && fullName !== 'null' && fullName !== 'undefined' && fullName !== '')
        ? fullName
        : (firstName && firstName !== 'null' && firstName !== 'undefined' && firstName !== '')
            ? firstName
            : null;

    if (displayName) {
        if (nameEl) nameEl.textContent = displayName;
        if (avatarEl) {
            const initials = displayName.substring(0, 1) +
                (displayName.indexOf(' ') > 0 ? displayName.substring(displayName.indexOf(' ') + 1, displayName.indexOf(' ') + 2) : '');
            avatarEl.textContent = initials;
        }
        if (welcomeEl) {
            welcomeEl.innerHTML = prefix + '<span style="font-weight: 800; color: var(--text-main);">' + displayName + '</span>';
        }
    }
}


document.addEventListener('DOMContentLoaded', () => {
    injectSharedHTML();
});