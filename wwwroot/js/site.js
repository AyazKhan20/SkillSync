// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.


    document.addEventListener('DOMContentLoaded', function() {
            // Mobile menu toggle
            const mobileMenuButton = document.getElementById('mobile-menu-button');
    const mobileMenu = document.getElementById('mobile-menu');
            mobileMenuButton.addEventListener('click', () => {
        mobileMenu.classList.toggle('hidden');
            });

    // Fade-in sections on scroll
    const sections = document.querySelectorAll('.fade-in-section');
            const observer = new IntersectionObserver((entries) => {
        entries.forEach(entry => {
            if (entry.isIntersecting) {
                entry.target.classList.add('is-visible');
            }
        });
            }, {
        threshold: 0.1
            });

            sections.forEach(section => {
        observer.observe(section);
            });

    // Gemini API Integration
    const summarizeBtn = document.getElementById('summarize-btn');
    const jobDescriptionInput = document.getElementById('job-description');
    const summaryResultDiv = document.getElementById('summary-result');
    const summaryLoader = document.getElementById('summary-loader');

    const generatePathBtn = document.getElementById('generate-path-btn');
    const jobTitleInput = document.getElementById('job-title-input');
    const pathResultDiv = document.getElementById('path-result');
    const pathLoader = document.getElementById('path-loader');
            
            const callGemini = async (prompt, resultDiv, loaderDiv) => {
        loaderDiv.classList.remove('hidden');
    resultDiv.classList.add('hidden');

    try {
        let chatHistory = [{role: "user", parts: [{text: prompt }] }];
    const payload = {contents: chatHistory };
        const apiKey = ""; // API key is handled by the environment
    const apiUrl = `https://generativelanguage.googleapis.com/v1beta/models/gemini-2.0-flash:generateContent?key=${apiKey}`;

    const response = await fetch(apiUrl, {
        method: 'POST',
    headers: {'Content-Type': 'application/json' },
    body: JSON.stringify(payload)
                    });

    if (!response.ok) {
                        throw new Error(`API call failed with status: ${response.status}`);
                    }

    const result = await response.json();

                    if (result.candidates && result.candidates.length > 0 &&
    result.candidates[0].content && result.candidates[0].content.parts &&
                        result.candidates[0].content.parts.length > 0) {
                        const text = result.candidates[0].content.parts[0].text;
    resultDiv.textContent = text;
                    } else {
                        throw new Error("Invalid response structure from API.");
                    }

                } catch (error) {
        console.error("Error calling Gemini API:", error);
    resultDiv.textContent = "Sorry, something went wrong. Please try again later.";
                } finally {
        loaderDiv.classList.add('hidden');
    resultDiv.classList.remove('hidden');
                }
            };

            summarizeBtn.addEventListener('click', () => {
                const jobDescription = jobDescriptionInput.value;
    if (jobDescription.trim() === "") {
        alert("Please paste a job description first.");
    return;
                }
    const prompt = `Summarize the following job description into two clear, bulleted lists: "Key Responsibilities" and "Required Skills". Be concise and focus on the most important points for a job seeker.\n\nDescription:\n${jobDescription}`;
    callGemini(prompt, summaryResultDiv, summaryLoader);
            });

            generatePathBtn.addEventListener('click', () => {
                const jobTitle = jobTitleInput.value;
    if (jobTitle.trim() === "") {
        alert("Please enter a job title.");
    return;
                }
    const prompt = `Create a clear, step-by-step career path roadmap for a student or fresher aspiring to become a "${jobTitle}". The roadmap should be encouraging and actionable. Organize it into the following sections:\n\n1.  ** foundational Skills:** (List 3-4 core skills to master first).\n2.  **Key Technologies & Tools:** (List the most important software, languages, or tools to learn).\n3.  **Project Ideas for Your Portfolio:** (Suggest 2-3 practical project ideas to build and showcase skills).\n4.  **Recommended Certifications:** (Suggest 1-2 valuable certifications to consider).\n5.  **Next Steps:** (Briefly describe how to start applying for internships or junior roles).`;
    callGemini(prompt, pathResultDiv, pathLoader);
            });
        });


//Analyze Page Javascript code


        document.addEventListener('DOMContentLoaded', () => {
            const fileUploadArea = document.getElementById('file-upload-area');
    const resumeFileInput = document.getElementById('resume-file-input');
    const fileNameDisplay = document.getElementById('file-name');
    const dreamJobInput = document.getElementById('dream-job-input');
    const analyzeBtn = document.getElementById('analyze-btn');
    const loader = document.getElementById('loader');
    const resultsSection = document.getElementById('results-section');
    const resultsContainer = document.getElementById('results-container');

    let fileIsSelected = false;

            fileUploadArea.addEventListener('click', () => resumeFileInput.click());

            ['dragenter', 'dragover', 'dragleave', 'drop'].forEach(eventName => {
        fileUploadArea.addEventListener(eventName, preventDefaults, false);
            });

    function preventDefaults(e) {
        e.preventDefault();
    e.stopPropagation();
            }

            ['dragenter', 'dragover'].forEach(eventName => {
        fileUploadArea.addEventListener(eventName, () => fileUploadArea.classList.add('drag-over'), false);
            });

            ['dragleave', 'drop'].forEach(eventName => {
        fileUploadArea.addEventListener(eventName, () => fileUploadArea.classList.remove('drag-over'), false);
            });

    fileUploadArea.addEventListener('drop', handleDrop, false);
    resumeFileInput.addEventListener('change', handleFileSelect, false);

    function handleDrop(e) {
        let dt = e.dataTransfer;
    let files = dt.files;
    handleFiles(files);
            }

    function handleFileSelect(e) {
        handleFiles(e.target.files);
            }

    function handleFiles(files) {
                if (files.length > 0) {
                    const file = files[0];
    if (file.type === "application/pdf") {
        fileNameDisplay.textContent = `File selected: ${file.name}`;
    fileIsSelected = true;
                    } else {
        alert("Please upload a PDF file.");
    fileNameDisplay.textContent = "";
    fileIsSelected = false;
                    }
                }
            }

            const callGemini = async (prompt) => {
                const apiKey = ""; // Handled by environment
    const apiUrl = `https://generativelanguage.googleapis.com/v1beta/models/gemini-2.0-flash:generateContent?key=${apiKey}`;
    const payload = {contents: [{role: "user", parts: [{text: prompt }] }] };

    const response = await fetch(apiUrl, {
        method: 'POST',
    headers: {'Content-Type': 'application/json' },
    body: JSON.stringify(payload)
                });

    if (!response.ok) throw new Error(`API Error: ${response.status}`);

    const result = await response.json();

    if (result.candidates && result.candidates[0].content.parts) {
                    return result.candidates[0].content.parts[0].text;
                } else {
                    throw new Error("Invalid API response structure.");
                }
            };

            analyzeBtn.addEventListener('click', async () => {
                const dreamJob = dreamJobInput.value;
    if (!fileIsSelected) {
        alert("Please upload your resume PDF first.");
    return;
                }
    if (dreamJob.trim() === "") {
        alert("Please enter your dream job title.");
    return;
                }

    loader.classList.remove('hidden');
    resultsSection.classList.add('hidden');

    // ** SIMULATION NOTE **
    // In a real application, you would send the PDF file to the backend here.
    // The backend would parse it (e.g., using iTextSharp in C#) to get the text.
    // For this frontend demo, we'll use mock resume text to simulate the parsed content.
    const mockResumeText = `
    John Doe - Aspiring Software Developer
    Email: john.doe@email.com | Phone: 123-456-7890

    Education:
    B.S. in Computer Science, State University (2020-2024)

    Skills:
    - Programming Languages: Python, JavaScript, Java
    - Frontend: HTML, CSS, React
    - Backend: Node.js, Express
    - Databases: SQL, MongoDB
    - Tools: Git, Docker, VS Code
    `;

    try {
                    const prompt = `A user has uploaded their resume. The extracted text is below. Their dream job is "${dreamJob}".

    Perform the following analysis and format the output using Markdown:

    1.  First, create a section "### Your Current Skills" and list the key skills extracted from the resume.
    2.  Second, create a section "### Key Skills for a ${dreamJob}" and list the top 5-7 most critical skills required for that role, based on general industry knowledge.
    3.  Third, create a section "### Your Skill Gap" and list ONLY the skills from the "${dreamJob}" list that are MISSING from the user's resume. If there are no gaps, state that.
    4.  Finally, create a section "### Recommended Courses to Fill the Gaps". For each missing skill, recommend one specific, high-quality online course. Include the course name and provider (like Coursera, Udemy, or Pluralsight). If there are no gaps, provide recommendations for advanced topics.

    Resume Text:
    ---
    ${mockResumeText}
    ---
    `;

    const analysisResult = await callGemini(prompt);
    displayResults(analysisResult);

                } catch (error) {
        console.error("Error during analysis:", error);
    resultsContainer.innerHTML = `<p class="text-red-500">An error occurred during analysis. Please try again later.</p>`;
                } finally {
        loader.classList.add('hidden');
    resultsSection.classList.remove('hidden');
                }
            });

    function displayResults(markdownText) {
        // A more robust markdown-to-HTML conversion
        let html = markdownText
    .split('### ')
                    .filter(section => section.trim() !== '')
                    .map(section => {
                        const lines = section.split(/\r?\n/).filter(line => line.trim() !== '');
    const title = lines.shift();
                        const listItems = lines.map(item => `<li>${item.replace(/^- /, '')}</li>`).join('');
    return `<h4>${title}</h4><ul>${listItems}</ul>`;
                    })
                    .join('');

                resultsContainer.innerHTML = html;
            }
        });
    
