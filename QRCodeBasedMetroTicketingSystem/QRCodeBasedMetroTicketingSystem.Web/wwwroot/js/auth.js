document.addEventListener('DOMContentLoaded', function() {
    const signupForm = document.getElementById('signupForm');
    const loginForm = document.getElementById('loginForm');
    const forgotPasswordForm = document.getElementById('forgotPasswordForm');
    const resetPasswordForm = document.getElementById('resetPasswordForm');

    if (signupForm) {
        const fullName = document.getElementById('fullName');
        const email = document.getElementById('email');
        const phone = document.getElementById('phone');
        const nid = document.getElementById('nid');
        const password = document.getElementById('password');
        const confirmPassword = document.getElementById('confirmPassword');
        const termsCheck = document.getElementById('termsCheck');

        // Add real-time validation for all fields
        fullName.addEventListener('input', validateField);
        email.addEventListener('input', validateField);
        phone.addEventListener('input', validateField);
        nid.addEventListener('input', validateField);
        password.addEventListener('input', validateField);
        confirmPassword.addEventListener('input', validateField);
        termsCheck.addEventListener('change', validateTerms);
    }

    if (loginForm) {
        const phone = document.getElementById('phone');
        phone.addEventListener('input', validateField);
    }

    if (forgotPasswordForm) {
        const email = document.getElementById('email');
        email.addEventListener('input', validateField);
    }

    if (resetPasswordForm) {
        const password = document.getElementById('password');
        const confirmPassword = document.getElementById('confirmPassword');
        password.addEventListener('input', validateField);
        confirmPassword.addEventListener('input', validateField);
    }

    // Field validation handler
    function validateField(e) {
        const field = e.target;
        const value = field.value;
        let isValid = false;
        
        switch(field.id) {
            case 'fullName':
                isValid = validateFullName(value);
                break;
            case 'email':
                isValid = validateEmail(value);
                break;
            case 'phone':
                isValid = validatePhone(value);
                break;
            case 'nid':
                isValid = validateNid(value);
                break;
            case 'password':
                isValid = validatePassword(value);
                break;
            case 'confirmPassword':
                isValid = validateConfirmPassword(value);
                break;
        }
        
        updateFieldStatus(field, isValid);
    }

    // Validation functions
    function validateFullName(value) {
        return /^[A-Za-z\s.]+$/.test(value.trim());
    }

    function validateEmail(value) {
        if (typeof value !== 'string' || value.length > 254) return false;
        return /^[a-zA-Z0-9.!#$%&'*+/=?^_`{|}~-]+@[a-zA-Z0-9](?:[a-zA-Z0-9-]*[a-zA-Z0-9])?(?:\.[a-zA-Z0-9](?:[a-zA-Z0-9-]*[a-zA-Z0-9])?)*\.[a-zA-Z]{2,63}$/i.test(value.trim());
    }

    function validatePhone(value) {
        return /^01[3-9]\d{8}$/.test(value.trim());
    }

    function validateNid(value) {
        return /^\d{10}$|^\d{17}$/.test(value.trim());
    }

    function validatePassword(value) {
        const hasNumber = /\d/.test(value);
        const hasLetter = /[A-Za-z]/.test(value);
        return value.length >= 8 && hasNumber && hasLetter;
    }

    function validateConfirmPassword(value) {
        return value === password.value;
    }

    function validateTerms() {
        const termsLabel = termsCheck.nextElementSibling;
        if (!termsCheck.checked) {
            termsLabel.style.color = '#dc3545';
        } else {
            termsLabel.style.color = '';
        }
        return termsCheck.checked;
    }

    // Update field visual status
    function updateFieldStatus(field, isValid) {
        field.classList.remove('is-valid', 'is-invalid');
        
        if (isValid) {
            field.classList.add('is-valid');
            const feedback = field.closest('div')?.querySelector('.invalid-feedback');
            if (feedback) feedback.textContent = '';
        } else {
            field.classList.add('is-invalid');
            let feedback = field.closest('div')?.querySelector('.invalid-feedback');
            if (!feedback) {
                feedback = document.createElement('div');
                feedback.className = 'invalid-feedback';
                field.closest('div').appendChild(feedback);
            }
            
            // Set appropriate error message
            switch(field.id) {
                case 'fullName':
                    feedback.textContent = 'Full Name must be non-empty and contain only letters, spaces, and periods';
                    break;
                case 'email':
                    feedback.textContent = 'Please enter a valid email address';
                    break;
                case 'phone':
                    feedback.textContent = 'Please enter a valid Bangladeshi phone number (11 digits starting with 01)';
                    break;
                case 'nid':
                    feedback.textContent = 'NID must be either 10 or 17 digits long and contain only numbers';
                    break;
                case 'password':
                    feedback.textContent = 'Password must be at least 8 characters with at least one letter and one number';
                    break;
                case 'confirmPassword':
                    feedback.textContent = 'Passwords do not match';
                    break;
            }
        }
    }

    // Sign up Form submission handler
    signupForm?.addEventListener('submit', function(event) {
        event.preventDefault();
        
        // Validate all fields
        const isFullNameValid = validateFullName(fullName.value);
        const isEmailValid = validateEmail(email.value);
        const isPhoneValid = validatePhone(phone.value);
        const isNidValid = validateNid(nid.value);
        const isPasswordValid = validatePassword(password.value);
        const isConfirmPasswordValid = validateConfirmPassword(confirmPassword.value);
        const isTermsChecked = validateTerms();

        // Update all field statuses
        updateFieldStatus(fullName, isFullNameValid);
        updateFieldStatus(email, isEmailValid);
        updateFieldStatus(phone, isPhoneValid);
        updateFieldStatus(nid, isNidValid);
        updateFieldStatus(password, isPasswordValid);
        updateFieldStatus(confirmPassword, isConfirmPasswordValid);

        // If all validations pass, submit the form
        if (isFullNameValid && isEmailValid && isPhoneValid && isNidValid && isPasswordValid && isConfirmPasswordValid && isTermsChecked) {
            signupForm.submit();
        }
    });

    // Login Form submission handler
    loginForm?.addEventListener('submit', function(event) {
        event.preventDefault();
        
        const isPhoneValid = validatePhone(phone.value);
        updateFieldStatus(phone, isPhoneValid);

        if (isPhoneValid) {
            loginForm.submit();
        }
    });

    // Password toggle visibility
    document.getElementById('passwordToggle')?.addEventListener('click', function() {
        const passwordInput = document.getElementById('password');
        const icon = this.querySelector('i');
        const PASSWORD_TYPE = "password";
        const TEXT_TYPE = "text";

        if (passwordInput.type === PASSWORD_TYPE) {
            passwordInput.type = TEXT_TYPE;
            icon.classList.remove("fa-eye");
            icon.classList.add("fa-eye-slash");
        } else {
            passwordInput.type = PASSWORD_TYPE;
            icon.classList.remove('fa-eye-slash');
            icon.classList.add('fa-eye');
        }
    });
});