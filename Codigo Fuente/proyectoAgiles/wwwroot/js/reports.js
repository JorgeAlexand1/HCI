// Funciones de utilidad para la página de reportes
window.ReportsUtils = {
    // Función para convertir datos a CSV
    convertToCSV: function(data) {
        if (!data || data.length === 0) return '';

        const headers = [
            'Docente',
            'Cédula',
            'Email',
            'Teléfono',
            'Facultad',
            'Carrera',
            'Nivel Actual',
            'Nivel Solicitado',
            'Años Experiencia',
            'Estado',
            'Fecha Solicitud',
            'Fecha Aprobación',
            'Observaciones'
        ];

        const csvRows = [];
        csvRows.push(headers.join(','));

        data.forEach(row => {
            const values = [
                `"${row.docenteNombre || ''}"`,
                `"${row.docenteCedula || ''}"`,
                `"${row.docenteEmail || ''}"`,
                `"${row.docenteTelefono || ''}"`,
                `"${row.facultad || ''}"`,
                `"${row.carrera || ''}"`,
                `"${row.nivelActual || ''}"`,
                `"${row.nivelSolicitado || ''}"`,
                `"${row.anosExperiencia || 0}"`,
                `"${row.status || ''}"`,
                `"${this.formatDate(row.fechaSolicitud)}"`,
                `"${this.formatDate(row.fechaAprobacion)}"`,
                `"${(row.observaciones || '').replace(/"/g, '""')}"` // Escape quotes in observations
            ];
            csvRows.push(values.join(','));
        });

        return csvRows.join('\n');
    },

    // Función para formatear fechas
    formatDate: function(dateString) {
        if (!dateString) return 'No especificada';
        try {
            const date = new Date(dateString);
            return date.toLocaleDateString('es-ES', {
                year: 'numeric',
                month: 'long',
                day: 'numeric'
            });
        } catch {
            return 'Fecha inválida';
        }
    },

    // Función para obtener la clase CSS del estado
    getStatusClass: function(status) {
        switch (status) {
            case 'Pendiente': return 'status-pending';
            case 'AprobadoConsejo':
            case 'Procesado': return 'status-approved';
            case 'RechazadoComision':
            case 'RechazadoConsejo': return 'status-rejected';
            case 'EnRevision':
            case 'Verificado': return 'status-review';
            default: return 'status-pending';
        }
    },

    // Función para obtener el texto del estado
    getStatusText: function(status) {
        switch (status) {
            case 'Pendiente': return 'Pendiente';
            case 'EnRevision': return 'En Revisión';
            case 'Verificado': return 'Verificado';
            case 'AprobadoPresidente': return 'Aprobado por Presidente';
            case 'AprobadoConsejo': return 'Aprobado por Consejo';
            case 'RechazadoComision': return 'Rechazado por Comisión';
            case 'RechazadoConsejo': return 'Rechazado por Consejo';
            case 'Procesado': return 'Procesado';
            case 'Finalizado': return 'Finalizado';
            default: return status;
        }
    },

    // Función para descargar archivos
    downloadFile: function(content, filename, contentType) {
        const blob = new Blob([content], { type: contentType });
        const link = document.createElement("a");
        
        if (link.download !== undefined) {
            const url = URL.createObjectURL(blob);
            link.setAttribute("href", url);
            link.setAttribute("download", filename);
            link.style.visibility = 'hidden';
            document.body.appendChild(link);
            link.click();
            document.body.removeChild(link);
            URL.revokeObjectURL(url);
        }
    }
};

// Funciones globales para compatibilidad con el código existente
window.downloadExcel = function (filename, data) {
    try {
        const csvContent = window.ReportsUtils.convertToCSV(data);
        window.ReportsUtils.downloadFile("\uFEFF" + csvContent, filename.replace('.xlsx', '.csv'), 'text/csv;charset=utf-8;');
    } catch (error) {
        console.error('Error downloading Excel file:', error);
        alert('Error al exportar el archivo. Por favor, intente nuevamente.');
    }
};

window.downloadTeacherReport = function (filename, solicitud) {
    try {
        const htmlContent = generateTeacherReportHTML(solicitud);
        window.ReportsUtils.downloadFile(htmlContent, filename.replace('.pdf', '.html'), 'text/html;charset=utf-8;');
    } catch (error) {
        console.error('Error downloading teacher report:', error);
        alert('Error al exportar la ficha del docente. Por favor, intente nuevamente.');
    }
};

function generateTeacherReportHTML(solicitud) {
    const cssStyles = `
    body {
        font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
        line-height: 1.6;
        color: #333;
        max-width: 800px;
        margin: 0 auto;
        padding: 20px;
        background-color: #f8f9fa;
    }
    .header {
        background: linear-gradient(135deg, #722f37 0%, #8b3a42 100%);
        color: white;
        padding: 30px;
        border-radius: 10px;
        text-align: center;
        margin-bottom: 30px;
        box-shadow: 0 4px 15px rgba(0,0,0,0.1);
    }
    .header h1 {
        margin: 0;
        font-size: 2rem;
        font-weight: 700;
    }
    .header p {
        margin: 10px 0 0 0;
        font-size: 1.1rem;
        opacity: 0.9;
    }
    .content {
        background: white;
        padding: 30px;
        border-radius: 10px;
        box-shadow: 0 4px 15px rgba(0,0,0,0.1);
    }
    .section {
        margin-bottom: 30px;
    }
    .section h2 {
        color: #722f37;
        border-bottom: 2px solid #722f37;
        padding-bottom: 10px;
        margin-bottom: 20px;
        font-size: 1.3rem;
    }
    .info-grid {
        display: grid;
        grid-template-columns: repeat(auto-fit, minmax(250px, 1fr));
        gap: 20px;
        margin-bottom: 20px;
    }
    .info-item {
        background: #f8f9fa;
        padding: 15px;
        border-radius: 8px;
        border-left: 4px solid #722f37;
    }
    .info-label {
        font-weight: 600;
        color: #722f37;
        margin-bottom: 5px;
        font-size: 0.9rem;
    }
    .info-value {
        font-size: 1rem;
        color: #333;
    }
    .status-badge {
        display: inline-block;
        padding: 8px 16px;
        border-radius: 20px;
        font-weight: 600;
        font-size: 0.9rem;
        text-transform: uppercase;
        letter-spacing: 0.5px;
    }
    .status-pending { background: #fff3cd; color: #856404; }
    .status-approved { background: #d4edda; color: #155724; }
    .status-rejected { background: #f8d7da; color: #721c24; }
    .status-review { background: #d1ecf1; color: #0c5460; }
    .observations {
        background: #f8f9fa;
        padding: 20px;
        border-radius: 8px;
        border-left: 4px solid #17a2b8;
        margin-top: 20px;
    }
    .footer {
        text-align: center;
        margin-top: 30px;
        padding: 20px;
        color: #6c757d;
        font-size: 0.9rem;
    }
    @media print {
        body { 
            background: white; 
        }
        .header, .content { 
            box-shadow: none; 
        }
    }
    `;

    return `
<!DOCTYPE html>
<html lang="es">
<head>
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>Ficha del Docente - ${solicitud.docenteNombre}</title>
    <style>${cssStyles}</style>
</head>
<body>
    <div class="header">
        <h1>Ficha del Docente</h1>
        <p>Universidad Técnica de Ambato</p>
    </div>

    <div class="content">
        <div class="section">
            <h2>Información Personal</h2>
            <div class="info-grid">
                <div class="info-item">
                    <div class="info-label">Nombre Completo</div>
                    <div class="info-value">${solicitud.docenteNombre}</div>
                </div>
                <div class="info-item">
                    <div class="info-label">Cédula</div>
                    <div class="info-value">${solicitud.docenteCedula}</div>
                </div>
                <div class="info-item">
                    <div class="info-label">Email</div>
                    <div class="info-value">${solicitud.docenteEmail}</div>
                </div>
                <div class="info-item">
                    <div class="info-label">Teléfono</div>
                    <div class="info-value">${solicitud.docenteTelefono || 'No especificado'}</div>
                </div>
            </div>
        </div>

        <div class="section">
            <h2>Información Académica</h2>
            <div class="info-grid">
                <div class="info-item">
                    <div class="info-label">Facultad</div>
                    <div class="info-value">${solicitud.facultad || 'No especificada'}</div>
                </div>
                <div class="info-item">
                    <div class="info-label">Carrera</div>
                    <div class="info-value">${solicitud.carrera || 'No especificada'}</div>
                </div>
                <div class="info-item">
                    <div class="info-label">Nivel Actual</div>
                    <div class="info-value">${solicitud.nivelActual}</div>
                </div>
                <div class="info-item">
                    <div class="info-label">Nivel Solicitado</div>
                    <div class="info-value">${solicitud.nivelSolicitado}</div>
                </div>
                <div class="info-item">
                    <div class="info-label">Años de Experiencia</div>
                    <div class="info-value">${solicitud.anosExperiencia} años</div>
                </div>
            </div>
        </div>

        <div class="section">
            <h2>Estado de la Solicitud</h2>
            <div class="info-grid">
                <div class="info-item">
                    <div class="info-label">Estado Actual</div>
                    <div class="info-value">
                        <span class="status-badge ${window.ReportsUtils.getStatusClass(solicitud.status)}">${window.ReportsUtils.getStatusText(solicitud.status)}</span>
                    </div>
                </div>
                <div class="info-item">
                    <div class="info-label">Fecha de Solicitud</div>
                    <div class="info-value">${window.ReportsUtils.formatDate(solicitud.fechaSolicitud)}</div>
                </div>
                ${solicitud.fechaAprobacion ? `
                <div class="info-item">
                    <div class="info-label">Fecha de Aprobación</div>
                    <div class="info-value">${window.ReportsUtils.formatDate(solicitud.fechaAprobacion)}</div>
                </div>
                ` : ''}
                ${solicitud.fechaRechazo ? `
                <div class="info-item">
                    <div class="info-label">Fecha de Rechazo</div>
                    <div class="info-value">${window.ReportsUtils.formatDate(solicitud.fechaRechazo)}</div>
                </div>
                ` : ''}
            </div>
        </div>

        ${(solicitud.observaciones || solicitud.motivoRechazo) ? `
        <div class="section">
            <h2>Observaciones</h2>
            ${solicitud.observaciones ? `
            <div class="observations">
                <div class="info-label">Observaciones Generales</div>
                <div class="info-value">${solicitud.observaciones}</div>
            </div>
            ` : ''}
            ${solicitud.motivoRechazo ? `
            <div class="observations">
                <div class="info-label">Motivo de Rechazo</div>
                <div class="info-value">${solicitud.motivoRechazo}</div>
            </div>
            ` : ''}
        </div>
        ` : ''}
    </div>

    <div class="footer">
        <p>Generado el ${new Date().toLocaleDateString('es-ES', { 
            year: 'numeric', 
            month: 'long', 
            day: 'numeric',
            hour: '2-digit',
            minute: '2-digit'
        })}</p>
        <p>Universidad Técnica de Ambato - Sistema de Gestión de Escalafón Docente</p>
    </div>
</body>
</html>`;
}

// Funciones para generar PDFs que se abran en nueva pestaña
window.generateAndOpenPDF = function(data, filename) {
    try {
        // Verificar si jsPDF está disponible
        if (typeof window.jsPDF === 'undefined') {
            console.warn('jsPDF no está disponible, generando HTML como alternativa');
            generateHTMLReport(data, filename);
            return;
        }

        const { jsPDF } = window.jsPDF;
        const doc = new jsPDF('l', 'mm', 'a4'); // Orientación horizontal para más espacio
        
        // Configurar fuente y colores
        doc.setFont('helvetica');
        
        // Título
        doc.setFontSize(16);
        doc.setTextColor(114, 47, 55); // Color principal del tema
        doc.text('Reporte de Solicitudes de Escalafón', 20, 20);
        
        // Fecha de generación
        doc.setFontSize(10);
        doc.setTextColor(100, 100, 100);
        doc.text(`Generado el: ${new Date().toLocaleDateString('es-ES')}`, 20, 30);
        
        // Configurar tabla
        const headers = [
            'Docente', 'Cédula', 'Nivel Actual', 'Nivel Solicitado', 
            'Estado', 'Fecha Solicitud'
        ];
        
        const rows = data.map(item => [
            item.docenteNombre || '',
            item.docenteCedula || '',
            item.nivelActual || '',
            item.nivelSolicitado || '',
            window.ReportsUtils.getStatusText(item.status) || '',
            window.ReportsUtils.formatDate(item.fechaSolicitud)
        ]);
        
        // Usar autoTable si está disponible
        if (doc.autoTable) {
            doc.autoTable({
                head: [headers],
                body: rows,
                startY: 40,
                styles: {
                    fontSize: 8,
                    cellPadding: 2
                },
                headStyles: {
                    fillColor: [114, 47, 55],
                    textColor: 255
                },
                alternateRowStyles: {
                    fillColor: [245, 245, 245]
                }
            });
        } else {
            // Fallback básico sin autoTable
            let yPosition = 40;
            doc.setFontSize(8);
            
            // Headers
            doc.setTextColor(0, 0, 0);
            headers.forEach((header, index) => {
                doc.text(header, 20 + (index * 40), yPosition);
            });
            
            yPosition += 10;
            
            // Datos
            rows.slice(0, 20).forEach((row, rowIndex) => { // Limitar a 20 filas para evitar overflow
                row.forEach((cell, cellIndex) => {
                    const text = String(cell).substring(0, 15); // Truncar texto largo
                    doc.text(text, 20 + (cellIndex * 40), yPosition + (rowIndex * 8));
                });
            });
        }
        
        // Abrir PDF en nueva pestaña
        const pdfBlob = doc.output('blob');
        const pdfUrl = URL.createObjectURL(pdfBlob);
        window.open(pdfUrl, '_blank');
        
        // Limpiar la URL después de un tiempo
        setTimeout(() => URL.revokeObjectURL(pdfUrl), 10000);
        
    } catch (error) {
        console.error('Error generating PDF:', error);
        // Fallback a HTML
        try {
            generateHTMLReport(data, filename);
        } catch (htmlError) {
            console.error('Error generating HTML fallback:', htmlError);
            alert('Error al generar el reporte. Por favor, intente nuevamente.');
        }
    }
};

window.generateTeacherPDF = function(solicitud, filename) {
    try {
        if (typeof window.jsPDF === 'undefined') {
            console.warn('jsPDF no está disponible, generando HTML como alternativa');
            const htmlContent = generateTeacherReportHTML(solicitud);
            openContentInNewTab(htmlContent, `${filename}.html`, 'text/html');
            return;
        }

        const { jsPDF } = window.jsPDF;
        const doc = new jsPDF('p', 'mm', 'a4'); // Orientación vertical
        
        doc.setFont('helvetica');
        
        // Header
        doc.setFillColor(114, 47, 55);
        doc.rect(0, 0, 210, 40, 'F');
        
        doc.setTextColor(255, 255, 255);
        doc.setFontSize(20);
        doc.text('Ficha del Docente', 105, 20, { align: 'center' });
        doc.setFontSize(12);
        doc.text('Universidad Técnica de Ambato', 105, 30, { align: 'center' });
        
        // Información personal
        let yPos = 60;
        doc.setTextColor(114, 47, 55);
        doc.setFontSize(14);
        doc.text('Información Personal', 20, yPos);
        
        yPos += 10;
        doc.setTextColor(0, 0, 0);
        doc.setFontSize(10);
        
        const personalInfo = [
            ['Nombre:', solicitud.docenteNombre],
            ['Cédula:', solicitud.docenteCedula],
            ['Email:', solicitud.docenteEmail],
            ['Teléfono:', solicitud.docenteTelefono || 'No especificado']
        ];
        
        personalInfo.forEach(([label, value]) => {
            doc.setFont('helvetica', 'bold');
            doc.text(label, 20, yPos);
            doc.setFont('helvetica', 'normal');
            doc.text(value, 60, yPos);
            yPos += 8;
        });
        
        // Información académica
        yPos += 10;
        doc.setTextColor(114, 47, 55);
        doc.setFontSize(14);
        doc.text('Información Académica', 20, yPos);
        
        yPos += 10;
        doc.setTextColor(0, 0, 0);
        doc.setFontSize(10);
        
        const academicInfo = [
            ['Facultad:', solicitud.facultad || 'No especificada'],
            ['Carrera:', solicitud.carrera || 'No especificada'],
            ['Nivel Actual:', solicitud.nivelActual],
            ['Nivel Solicitado:', solicitud.nivelSolicitado],
            ['Años de Experiencia:', `${solicitud.anosExperiencia} años`]
        ];
        
        academicInfo.forEach(([label, value]) => {
            doc.setFont('helvetica', 'bold');
            doc.text(label, 20, yPos);
            doc.setFont('helvetica', 'normal');
            doc.text(value, 60, yPos);
            yPos += 8;
        });
        
        // Estado de la solicitud
        yPos += 10;
        doc.setTextColor(114, 47, 55);
        doc.setFontSize(14);
        doc.text('Estado de la Solicitud', 20, yPos);
        
        yPos += 10;
        doc.setTextColor(0, 0, 0);
        doc.setFontSize(10);
        
        const statusInfo = [
            ['Estado:', window.ReportsUtils.getStatusText(solicitud.status)],
            ['Fecha de Solicitud:', window.ReportsUtils.formatDate(solicitud.fechaSolicitud)]
        ];
        
        if (solicitud.fechaAprobacion) {
            statusInfo.push(['Fecha de Aprobación:', window.ReportsUtils.formatDate(solicitud.fechaAprobacion)]);
        }
        
        statusInfo.forEach(([label, value]) => {
            doc.setFont('helvetica', 'bold');
            doc.text(label, 20, yPos);
            doc.setFont('helvetica', 'normal');
            doc.text(value, 60, yPos);
            yPos += 8;
        });
        
        // Observaciones
        if (solicitud.observaciones) {
            yPos += 10;
            doc.setTextColor(114, 47, 55);
            doc.setFontSize(14);
            doc.text('Observaciones', 20, yPos);
            
            yPos += 10;
            doc.setTextColor(0, 0, 0);
            doc.setFontSize(10);
            
            const lines = doc.splitTextToSize(solicitud.observaciones, 170);
            doc.text(lines, 20, yPos);
        }
        
        // Footer
        doc.setTextColor(100, 100, 100);
        doc.setFontSize(8);
        doc.text(`Generado el ${new Date().toLocaleDateString('es-ES')}`, 105, 280, { align: 'center' });
        
        // Abrir PDF en nueva pestaña
        const pdfBlob = doc.output('blob');
        const pdfUrl = URL.createObjectURL(pdfBlob);
        window.open(pdfUrl, '_blank');
        
        // Limpiar la URL después de un tiempo
        setTimeout(() => URL.revokeObjectURL(pdfUrl), 10000);
        
    } catch (error) {
        console.error('Error generating teacher PDF:', error);
        // Fallback a HTML
        try {
            const htmlContent = generateTeacherReportHTML(solicitud);
            openContentInNewTab(htmlContent, `${filename}.html`, 'text/html');
        } catch (htmlError) {
            console.error('Error generating HTML fallback:', htmlError);
            alert('Error al generar la ficha del docente. Por favor, intente nuevamente.');
        }
    }
};

// Función auxiliar para generar reporte HTML
function generateHTMLReport(data, filename) {
    const htmlContent = `
    <!DOCTYPE html>
    <html lang="es">
    <head>
        <meta charset="UTF-8">
        <meta name="viewport" content="width=device-width, initial-scale=1.0">
        <title>Reporte de Solicitudes de Escalafón</title>
        <style>
            body { 
                font-family: Arial, sans-serif; 
                margin: 20px; 
                line-height: 1.6; 
            }
            .header { 
                text-align: center; 
                margin-bottom: 30px; 
                color: #722f37; 
            }
            table { 
                width: 100%; 
                border-collapse: collapse; 
                margin-top: 20px; 
            }
            th, td { 
                border: 1px solid #ddd; 
                padding: 8px; 
                text-align: left; 
            }
            th { 
                background-color: #722f37; 
                color: white; 
            }
            tr:nth-child(even) { 
                background-color: #f2f2f2; 
            }
        </style>
    </head>
    <body>
        <div class="header">
            <h1>Reporte de Solicitudes de Escalafón</h1>
            <p>Universidad Técnica de Ambato</p>
            <p>Generado el: ${new Date().toLocaleDateString('es-ES')}</p>
        </div>
        <table>
            <thead>
                <tr>
                    <th>Docente</th>
                    <th>Cédula</th>
                    <th>Nivel Actual</th>
                    <th>Nivel Solicitado</th>
                    <th>Estado</th>
                    <th>Fecha Solicitud</th>
                </tr>
            </thead>
            <tbody>
                ${data.map(item => `
                    <tr>
                        <td>${item.docenteNombre || ''}</td>
                        <td>${item.docenteCedula || ''}</td>
                        <td>${item.nivelActual || ''}</td>
                        <td>${item.nivelSolicitado || ''}</td>
                        <td>${window.ReportsUtils.getStatusText(item.status) || ''}</td>
                        <td>${window.ReportsUtils.formatDate(item.fechaSolicitud)}</td>
                    </tr>
                `).join('')}
            </tbody>
        </table>
    </body>
    </html>`;
    
    openContentInNewTab(htmlContent, `${filename}.html`, 'text/html');
}

// Función auxiliar para abrir contenido en nueva pestaña
function openContentInNewTab(content, filename, contentType) {
    const blob = new Blob([content], { type: contentType });
    const url = URL.createObjectURL(blob);
    const newTab = window.open(url, '_blank');
    
    if (!newTab) {
        // Si el popup fue bloqueado, descargar el archivo
        const link = document.createElement('a');
        link.href = url;
        link.download = filename;
        document.body.appendChild(link);
        link.click();
        document.body.removeChild(link);
    }
    
    // Limpiar la URL después de un tiempo
    setTimeout(() => URL.revokeObjectURL(url), 10000);
}
