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
