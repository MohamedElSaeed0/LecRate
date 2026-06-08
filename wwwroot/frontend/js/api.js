

const API_URL = '/api';



async function apiCall(endpoint, method = 'GET', data = null) {
    const options = {
        method: method,
        headers: {
            'Content-Type': 'application/json'
        }
    };

    
    const token = localStorage.getItem('token');
    if (token) {
        options.headers['Authorization'] = `Bearer ${token}`;
    }

    
    if (data !== null && data !== undefined) {
        options.body = JSON.stringify(data);
    }

    try {
        const response = await fetch(`${API_URL}${endpoint}`, options);

        
        if (!response.ok) {
            
            if (response.status === 401) {
                if (!endpoint.includes('/auth/login')) {
                    localStorage.clear();
                    window.location.href = '/frontend/login.html';
                    return { success: false, data: { message: 'انتهت الجلسة' } };
                }
            }

            try {
                const errJson = await response.json();
                console.error('API Error Details:', errJson);
                return { success: false, data: errJson };
            } catch {
                
                console.error('API Error:', response.status, response.statusText);
                return { success: false, data: { message: `خطأ في الخادم: ${response.status}` } };
            }
        }

        const result = await response.json();
        return { success: true, data: result };

    } catch (error) {
        console.error('API Network Error:', error);

        if (error.message === 'Failed to fetch') {
            return { success: false, data: { message: 'لا يمكن الاتصال بالسيرفر' } };
        }
        return { success: false, data: { message: error.message || 'حدث خطأ غير متوقع' } };
    }
}


const AuthAPI = {
    login: (username, password, userType) => {
        return apiCall('/auth/login', 'POST', { username, password, userType });
    }
};


const StudentsAPI = {
    getAll: (page = 1, pageSize = 20) => apiCall(`/students/GetAll?page=${page}&pageSize=${pageSize}`),
    search: (query) => apiCall(`/students/Search?query=${encodeURIComponent(query)}`),
    getById: (id) => apiCall(`/students/GetById/${id}`),
    add: (data) => apiCall('/students/Add', 'POST', data),
    update: (id, data) => apiCall(`/students/Update/${id}`, 'PUT', data),
    delete: (id) => apiCall(`/students/Delete/${id}`, 'DELETE')
};


const StudentSubjectsAPI = {
    getAll: () => apiCall('/studentsubjects/GetAll'),
    getByStudent: (id) => apiCall(`/studentsubjects/GetByStudent/${id}`),
    getBySubject: (id) => apiCall(`/studentsubjects/GetBySubject/${id}`),
    add: (data) => apiCall('/studentsubjects/Add', 'POST', data),
    update: (id, data) => apiCall(`/studentsubjects/Update/${id}`, 'PUT', data),
    delete: (id) => apiCall(`/studentsubjects/Delete/${id}`, 'DELETE')
};


const LecturersAPI = {
    getAll: () => apiCall('/lecturers/GetAll'),
    search: (query) => apiCall(`/lecturers/Search?query=${encodeURIComponent(query)}`),
    getById: (id) => apiCall(`/lecturers/GetById/${id}`),
    add: (data) => apiCall('/lecturers/Add', 'POST', data),
    update: (id, data) => apiCall(`/lecturers/Update/${id}`, 'PUT', data),
    delete: (id) => apiCall(`/lecturers/Delete/${id}`, 'DELETE')
};


const LecturerSubjectsAPI = {
    getAll: () => apiCall('/lecturersubjects/GetAll'),
    getByLecturer: (id) => apiCall(`/lecturersubjects/GetByLecturer/${id}`),
    getBySubject: (id) => apiCall(`/lecturersubjects/GetBySubject/${id}`),
    add: (data) => apiCall('/lecturersubjects/Add', 'POST', data),
    update: (id, data) => apiCall(`/lecturersubjects/Update/${id}`, 'PUT', data),
    delete: (id) => apiCall(`/lecturersubjects/Delete/${id}`, 'DELETE')
};


const QuestionsAPI = {
    getAll: () => apiCall('/questions/GetAll'),
    add: (data) => apiCall('/questions/Add', 'POST', data),
    update: (id, data) => apiCall(`/questions/Update/${id}`, 'PUT', data),
    delete: (id) => apiCall(`/questions/Delete/${id}`, 'DELETE')
};


const SubjectsAPI = {
    getAll: () => apiCall('/subjects/GetAll'),
    add: (data) => apiCall('/subjects/Add', 'POST', data),
    update: (id, data) => apiCall(`/subjects/Update/${id}`, 'PUT', data),
    delete: (id) => apiCall(`/subjects/Delete/${id}`, 'DELETE')
};


const EvaluationsAPI = {
    getAll: () => apiCall('/evaluations/GetAll'),
    getReport: () => apiCall('/evaluations/GetReport'),
    add: (data) => apiCall('/evaluations/Add', 'POST', data)
};




function showAlert(message, type = 'success') {
    const alertDiv = document.createElement('div');
    alertDiv.className = `alert alert-${type}`;
    alertDiv.textContent = message;

    const container = document.querySelector('.main-content') || document.body;
    container.insertBefore(alertDiv, container.firstChild);

    setTimeout(() => alertDiv.remove(), 3000);
}


function checkAuth() {
    const token = localStorage.getItem('token');
    const userType = localStorage.getItem('userType');

    if (!token) {
        window.location.href = '/frontend/login.html';
        return false;
    }
    return { token, userType };
}


function logout() {
    localStorage.removeItem('token');
    localStorage.removeItem('userType');
    localStorage.removeItem('userId');
    window.location.href = '/frontend/login.html';
}


function setupSearchDropdown(inputId, listId, apiSearchFn, onSelect) {
    const input = document.getElementById(inputId);
    const list = document.getElementById(listId);
    let timeout = null;

    input.addEventListener('input', function () {
        const query = this.value;
        if (query.length < 1) {
            list.style.display = 'none';
            return;
        }

        clearTimeout(timeout);
        timeout = setTimeout(async () => {
            const result = await apiSearchFn(query);
            if (result.success && result.data.length > 0) {
                list.innerHTML = result.data.map(item =>
                    `<div class="search-item" data-id="${item.studentId || item.lecturerId}">
                        ${item.firstName} ${item.lastName} (${item.username})
                    </div>`
                ).join('');
                list.style.display = 'block';

                
                document.querySelectorAll(`#${listId} .search-item`).forEach(div => {
                    div.addEventListener('click', function () {
                        input.value = this.innerText;
                        list.style.display = 'none';
                        onSelect(this.getAttribute('data-id'));
                    });
                });
            } else {
                list.innerHTML = '<div class="search-item disabled">لا توجد نتائج</div>';
                list.style.display = 'block';
            }
        }, 300); 
    });

    
    document.addEventListener('click', function (e) {
        if (e.target !== input && e.target !== list) {
            list.style.display = 'none';
        }
    });
}